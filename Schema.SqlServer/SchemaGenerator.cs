using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Schema.Common;
using Schema.Common.Connectivity;
using Schema.Common.Interfaces;
using Schema.Common.SchemaObjects;

namespace Schema.SqlServer
{
    public partial class SchemaGenerator : ISchemaGenerator
    {
        public DbSchema GetSchema(DatabaseConnectionInfo connectionInfo)
        {
            var dbObjects = new List<DbSchemaObject>();
            var tables = GetTables(connectionInfo);
            foreach (var dbObject in tables.Values)
                dbObjects.Add(dbObject);

            var modules = GetModules(connectionInfo);
            foreach (var dbObject in modules)
                dbObjects.Add(dbObject);

            var fkeys = GetForeignKeys(connectionInfo);
            foreach (var dbObject in fkeys )
                dbObjects.Add(dbObject);

            var schema = new DbSchema(dbObjects);
            return schema;
        }


        private List<  DbForeignKey> GetForeignKeys(DatabaseConnectionInfo connectionInfo)
        {
            var fKeys = new Dictionary<string, DbForeignKey>();

            var sql = @"select	SCHEMA_NAME(sof.schema_id) + '.' + sof.name as ConstraintName ,
        SCHEMA_NAME(sof.schema_id) as SchemaName ,
		sof.name,
		SCHEMA_NAME(sop.schema_id) + '.' + sop.name as ForeignKeyTable, 
		SCHEMA_NAME(sor.schema_id) + '.' +sor.name  as PrimaryKeyTable, 
		scp.name as ForeignKeyColumn, 
		scf.name as PrimaryKeyColumn
from sys.foreign_key_columns  fk

inner join Sys.objects sop on fk.parent_object_id = sop.object_id
inner join Sys.objects sof on fk.constraint_object_id = sof.object_id
inner join Sys.objects sor on fk.referenced_object_id = sor.object_id
inner join Sys.Columns scp on fk.parent_column_id = scp.column_id and fk.parent_object_id = scp.object_id
inner join Sys.Columns scf on fk.referenced_column_id = scf.column_id and fk.referenced_object_id = scf.object_id";
            var primaryKeys = new Dictionary<string, List<string>>();

            using (var conn = new SqlConnection(connectionInfo.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    var reader = cmd.ExecuteReader();
                    var constraintNamePos = reader.GetOrdinal("ConstraintName");
                    var schemaNamePos = reader.GetOrdinal("SchemaName");
                    var namePos = reader.GetOrdinal("name");
                    var foreignKeyTablePos = reader.GetOrdinal("ForeignKeyTable");
                    var primaryKeyTablePos = reader.GetOrdinal("PrimaryKeyTable");
                    var foreignKeyColumnPos = reader.GetOrdinal("ForeignKeyColumn");
                    var primaryKeyColumnPos = reader.GetOrdinal("PrimaryKeyColumn");
                    while (reader.Read())
                    {
                        var constraintName = reader.GetString(constraintNamePos);
                        DbForeignKey fKey;
                        if (!fKeys.ContainsKey(constraintName))
                        {
                            fKey = new DbForeignKey
                            {
                                ConstraintFullName = constraintName,
                                Name = reader.GetString(namePos),
                                SchemaName= reader.GetString( schemaNamePos),
                                ForeignKeyTable = reader.GetString(foreignKeyTablePos),
                                PrimaryKeyTable = reader.GetString(primaryKeyTablePos)
                            };
                            fKeys.Add(constraintName, fKey);
                        }
                        fKeys[constraintName].Columns.Add(new DbForeignKeyColumn
                        {
                            PrimaryKeyColumn = reader.GetString(primaryKeyColumnPos),
                            ForeignKeyColumn = reader.GetString(foreignKeyColumnPos),
                        });
                    }
                }
            }
            Dictionary<string, DbForeignKey>.ValueCollection fkList = fKeys.Values;


            foreach (var key in  fkList)
            {
                CodeBuilder cb = new CodeBuilder();
                cb.AppendLine($"alter table {key.ForeignKeyTable}");
                cb.Indent();
                cb.AppendLine($"Add constraint {key.Name}");
                cb.StartLine("foreign key(");
                cb.AppendDelimited(", ", key.Columns, c => c.ForeignKeyColumn);
                cb.Append($") references {key.PrimaryKeyTable}(");
                cb.AppendDelimited(", ", key.Columns, c => c.PrimaryKeyColumn);
                cb.EndLine(")");
                key.Definition = cb.ToString();
            }

            return fkList.ToList();
        }

        public List<DbSchemaObject> GetModules(DatabaseConnectionInfo connectionInfo)
        {
            var moduleCommand = GetModulesCommand();
            return GetModules(connectionInfo, moduleCommand);

        }

        private SqlCommand GetModulesCommand()
        {
            var cmd = new SqlCommand
            {
                CommandType = CommandType.Text,
                CommandText =
                    @"select SCHEMA_NAME(p.schema_id) + '.' +  p.name as fullname,p.name ,SCHEMA_NAME(p.schema_id) as schemaName, m.definition , p.type
from sys.objects p inner join
 sys.sql_modules m on p.object_id = m.object_id"
            };

            return cmd;
        }


        private List<DbSchemaObject> GetModules(DatabaseConnectionInfo connectionInfo,
            SqlCommand getModulesCommand)
        {
            List<DbSchemaObject> objects = new List<DbSchemaObject>();
            using (SqlConnection conn = new SqlConnection(connectionInfo.ConnectionString))
            {
                conn.Open();

                //Get tables 
                getModulesCommand.Connection = conn;
                using (var reader = getModulesCommand.ExecuteReader())
                {
                    var fullNamePos = reader.GetOrdinal("fullName");
                    var namePos = reader.GetOrdinal("name");
                    var schemaNamePos = reader.GetOrdinal("schemaName");
                    var typePos = reader.GetOrdinal("type");
                    var definitionPos = reader.GetOrdinal("definition");
                    while (reader.Read())
                    {
                        var sqlServerType = reader.GetString(typePos).Trim();
                        DbSchemaObject dbObject = NewObjectOfType(sqlServerType);
                        dbObject.Name = reader.GetString(namePos);
                        dbObject.SchemaName = reader.GetString(schemaNamePos);
                        dbObject.Definition = reader.GetString(definitionPos).Trim();

                        objects.Add(dbObject);
                    }
                }

                return objects;
            }
        }

        private static DbSchemaObject NewObjectOfType(string sqlServerType)
        {

            switch (sqlServerType)
            {
                case "V":
                    return new DbView();
                    break;
                case "P":
                    return new DbStoredProc();
                    break;
                case "FN":
                    return new DbScalarFunction();
                    break;
                case "IF":
                case "TF":
                    return new DbTableFunction();
                    break;

                case "TR":
                    return new DbTrigger();
                    break;

                default:
                    throw new Exception($"Object of type {sqlServerType} encountered");
                    break;
            }

        }

        public Dictionary<string, DbView> GetViews(DatabaseConnectionInfo connectionInfo)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, DbView> GetViews(DatabaseConnectionInfo connectionInfo, string name)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<DbColumn>> GetViewColumns(DatabaseConnectionInfo connectionInfo, string name)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<DbColumn>> GetViewColumns(DatabaseConnectionInfo connectionInfo)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, DbStoredProc> GetStoredProcedures(DatabaseConnectionInfo connectionInfo)
        {
            var sql = @"select SCHEMA_NAME(p.schema_id) + '.' +  p.name as fullname,p.name ,SCHEMA_NAME(p.schema_id) as schemaName, m.definition from sys.procedures p inner join
 sys.sql_modules m on p.object_id = m.object_id";

            var procedures = new Dictionary<string, DbStoredProc>();
            using (var conn = new SqlConnection(connectionInfo.ConnectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    var reader = cmd.ExecuteReader();
                    var fullNamePos = reader.GetOrdinal("fullName");
                    var namePos = reader.GetOrdinal("name");
                    var schemaNamePos = reader.GetOrdinal("schemaName");
                    var definitionPos = reader.GetOrdinal("definition");

                    while (reader.Read())
                    {

                        procedures.Add(reader.GetString(fullNamePos), new DbStoredProc()
                        {
                            Name = reader.GetString(namePos),
                            SchemaName = reader.GetString(schemaNamePos),
                            Definition = reader.GetString(definitionPos).Trim()
                        });
                    }
                }
            }
            return procedures;
        }

        public DbStoredProc GetStoredProcedure(DatabaseConnectionInfo connectionInfo, string name)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<DbParameter>> GetStoredProcedureParameters(DatabaseConnectionInfo connectionInfo)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<DbParameter>> GetStoredProcedureParameters(DatabaseConnectionInfo connectionInfo, string name)
        {
            throw new NotImplementedException();
        }


    }
}
