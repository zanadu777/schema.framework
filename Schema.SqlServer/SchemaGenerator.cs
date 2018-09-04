using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema.Common.Connectivity;
using Schema.Common.Interfaces;
using Schema.Common.SchemaObjects;

namespace Schema.SqlServer
{
	public class SchemaGenerator : ISchemaGenerator
	{
		public DbSchema GetSchema(DatabaseConnectionInfo connectionInfo)
		{
			var dbObjects = new List<DbSchemaObject>();
			var tables = GetTables(connectionInfo);
			foreach (var dbObject in tables.Values)
				dbObjects.Add(dbObject);

		    var storedProcs = GetStoredProcedures(connectionInfo);
		    foreach (var dbObject in storedProcs.Values)
		        dbObjects.Add(dbObject);

            var schema = new DbSchema(dbObjects);
			return schema;
		}

		#region get tables 


		public Dictionary<string, DbTable> GetTables(DatabaseConnectionInfo connectionInfo)
		{
			var getTablesCommand = GetAllTablesCommand();
			var getColumnsCommand = GetAllColumnsCommand();
			var getPrimaryKeysCommand = GetAllPrimaryKeysCommand();
			var getPrimaryKeyColumnsCommand = GetAllPrimaryKeyColumnsCommand();

			var tables = GetTables(connectionInfo, getTablesCommand, getColumnsCommand, getPrimaryKeysCommand, getPrimaryKeyColumnsCommand);

			return tables;
		}

		private SqlCommand GetAllTablesCommand()
		{
			var cmd = new SqlCommand
			{
				CommandType = CommandType.Text,
				CommandText =
					@"select sc.Name + '.'+ so.name as fullName, so.name, sc.name as schemaName from sys.objects so
 inner join sys.schemas  sc on so.schema_id = sc.schema_id
where type in ('U')"
			};

			return cmd;
		}


		private SqlCommand GetAllColumnsCommand()
		{
			var cmd = new SqlCommand
			{
				CommandType = CommandType.Text,
				CommandText =
					@"select SCHEMA_NAME(so.schema_id) + '.'+ so.name as fullTableName ,c.name as columnName,c.column_id as ordinal, c.is_nullable ,
   c.is_identity, ISNULL(i.seed_value,0) as Seed, ISNULL(i.increment_value,0)  as Step,
   c.is_computed  ,TYPE_NAME(c.system_type_id) as datatype,c.max_length, c.precision
  from sys.columns c 
 inner join sys.objects so on so.object_id = c.object_id
 left outer Join sys.identity_columns i  on c.object_id = i.object_id
 where so.type in ('U') 
 order by so.object_id, c.column_id"
			};

			return cmd;
		}


		private SqlCommand GetAllPrimaryKeysCommand()
		{
			var cmd = new SqlCommand
			{
				CommandType = CommandType.Text,
				CommandText =
					@"SELECT 
OBJECT_SCHEMA_NAME(t.object_id)   + '.' + t.name  as fullTableName,
OBJECT_SCHEMA_NAME(t.object_id) AS schema_name
,t.name AS table_name
,i.index_id
,i.name AS index_name
,p.partition_number
,fg.name AS filegroup_name
, p.rows ,i.type_desc , i.is_padded, i.allow_page_locks, i.allow_row_locks, i.is_disabled
FROM sys.tables t
INNER JOIN sys.key_constraints k  on  k.parent_object_id = t.object_id
INNER JOIN sys.indexes i ON t.object_id = i.object_id and i.name = k.name
INNER JOIN sys.partitions p ON i.object_id=p.object_id AND i.index_id=p.index_id
LEFT OUTER JOIN sys.partition_schemes ps ON i.data_space_id=ps.data_space_id
LEFT OUTER JOIN sys.destination_data_spaces dds ON ps.data_space_id=dds.partition_scheme_id AND p.partition_number=dds.destination_id
INNER JOIN sys.filegroups fg ON COALESCE(dds.data_space_id, i.data_space_id)=fg.data_space_id"
			};

			return cmd;
		}

		private SqlCommand GetAllPrimaryKeyColumnsCommand()
		{
			var cmd = new SqlCommand
			{
				CommandType = CommandType.Text,
				CommandText =
					@"SELECT
SCHEMA_NAME(t.schema_id)  + '.' +    t.name     as tableFullName,t.name,SCHEMA_NAME(t.schema_id) as tableSchema,
	col_name(c.object_id, c.column_id)	AS columnName,
	c.key_ordinal				AS ORDINAL_POSITION , c.is_descending_key
FROM
	sys.key_constraints k JOIN sys.index_columns c
		ON c.object_id = k.parent_object_id
		AND c.index_id = k.unique_index_id
	JOIN sys.tables t ON t.object_id = k.parent_object_id"
			};

			return cmd;
		}


		private Dictionary<string, DbTable> GetTables(DatabaseConnectionInfo connectionInfo,
			SqlCommand getTablesCommand,
			SqlCommand getColumnsCommand,
			SqlCommand getPrimaryKeysCommand,
			SqlCommand getPrimaryKeyColumnsCommand
		)
		{
			var tables = new Dictionary<string, DbTable>();
			using (SqlConnection conn = new SqlConnection(connectionInfo.ConnectionString))
			{
				conn.Open();

				//Get tables 
				getTablesCommand.Connection = conn;
				using (var reader = getTablesCommand.ExecuteReader())
				{
					var fullNamePos = reader.GetOrdinal("fullName");
					var namePos = reader.GetOrdinal("name");
					var schemaNamePos = reader.GetOrdinal("schemaName");
					while (reader.Read())
					{
						tables.Add(reader.GetString(fullNamePos), new DbTable
						{
							Name = reader.GetString(namePos),
							SchemaName = reader.GetString(schemaNamePos)
						});
					}
				}

				//Get Columns
				getColumnsCommand.Connection = conn;

				using (var reader = getColumnsCommand.ExecuteReader())
				{
					var fullTableNamePos = reader.GetOrdinal("fullTableName");
					var columnNamePos = reader.GetOrdinal("columnName");
					var is_nullablePos = reader.GetOrdinal("is_nullable");
					var is_identityPos = reader.GetOrdinal("is_identity");
					var seedPos = reader.GetOrdinal("Seed");
					var stepPos = reader.GetOrdinal("Step");
					var datatypePos = reader.GetOrdinal("datatype");
					var max_lengthPos = reader.GetOrdinal("max_length");
					var ordinalPos = reader.GetOrdinal("ordinal");
					while (reader.Read())
					{
						var fullTableName = reader.GetString(fullTableNamePos);
						var table = tables[fullTableName];

						try
						{
							var col = new DbColumn
							{
								Name = reader.GetString(columnNamePos),
								IsNullable = reader.GetBoolean(is_nullablePos),
								IsIdentity = reader.GetBoolean(is_identityPos),
								IdentitySeed = reader.GetInt32(seedPos),
								IdentityStep = reader.GetInt32(stepPos),
								DataType = reader.GetString(datatypePos),
								MaxLength = (Int16) reader.GetValue(max_lengthPos),
								Ordinal = reader.GetInt32(ordinalPos)
							};

							table.Columns.Add(col);
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex.Message);
						}
					}
				}

				//Get primary keys 
				getPrimaryKeysCommand.Connection = conn;
				using (var reader = getPrimaryKeysCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						var fullTablename = (string) reader["fullTableName"];
						var table = tables[fullTablename];

						var pk = new DbPrimaryKey();
						pk.TableSchema = (string) reader["schema_name"];
						pk.Table = (string) reader["table_name"];
						pk.Name = (string) reader["index_name"];
						pk.Partition = (int) reader["partition_number"];
						pk.FileGroup = (string) reader["filegroup_name"];
						pk.RowCount = (long) reader["rows"];
						pk.Type = (string) reader["type_desc"];
						pk.IsPadded = (bool) reader["is_padded"];
						pk.IsAllowingPageLocks = (bool) reader["allow_page_locks"];
						pk.IsAllowingRowLocks = (bool) reader["allow_row_locks"];
						pk.IsDisabled = (bool) reader["is_disabled"];

						table.PrimaryKey = pk;
					}
				}

				//get primary Key columns
				getPrimaryKeyColumnsCommand.Connection = conn;
				using (var reader = getPrimaryKeyColumnsCommand.ExecuteReader())
				{
					var columnNamePos = reader.GetOrdinal("columnName");
					var tableFullName = reader.GetOrdinal("tableFullName");
					while (reader.Read())
					{
						var fullTablename =  reader.GetString(tableFullName);
						var table = tables[fullTablename];
						var columnName = reader.GetString(columnNamePos);

						table.Columns.Where(c => c.Name == columnName).First().IsInPrimaryKey = true;
					}
				}

				return tables;

			}
		}

		#endregion

		 
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
				var sql = @" select SCHEMA_NAME(p.schema_id) + '.' +  p.name as fullname,p.name ,SCHEMA_NAME(p.schema_id) as schemaName, m.definition from sys.procedures p inner join
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
