using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Errata.IO;
using Schema.Common.SchemaObjects;

namespace Schema.SqlServer
{
    public class SchemaFromFileSystem
    {
       
        public DbSchema GetSchema(DirectoryInfo directory)
        {
            var files = directory.GetFiles();
            return GetSchema(files);
        }

        public DbSchema GetSchema(IEnumerable<FileInfo> files)
        {
            var schema = new DbSchema();
            foreach (var file in files)
            {
                var schemaObject = ParseSchemaObject(file);
                schema.Add(schemaObject);
            }


            return schema;
        }


        private DbSchemaObject ParseSchemaObject(FileInfo file)
        {
            DbSchemaObject schemaObject = null;
            var name = System.IO.Path.GetFileNameWithoutExtension(file.FullName);
            var objectTypeName = name.Substring(0, name.IndexOf('_'));

            var objectType = (ESchemaObjectType)Enum.Parse(typeof(ESchemaObjectType), objectTypeName);
            switch (objectType)
            {
                case ESchemaObjectType.Table:
                    schemaObject = new DbTable();
                    break;
                case ESchemaObjectType.View:
                    schemaObject = new DbView();
                    break;
                case ESchemaObjectType.StoredProcedure:
                    schemaObject = new DbStoredProc();
                    break;
                case ESchemaObjectType.ScalarFunction:
                    schemaObject = new DbScalarFunction();
                    break;
                case ESchemaObjectType.TableFunction:
                    schemaObject = new DbTableFunction();
                    break;
                case ESchemaObjectType.Trigger:
                    schemaObject = new DbTrigger();
                    break;
                case ESchemaObjectType.PrimaryKey:
                    schemaObject = new DbPrimaryKey();
                    break;
                case ESchemaObjectType.ForeignKey:
                    schemaObject = new DbForeignKey();
                    break;
                case ESchemaObjectType.Index:
                    schemaObject = new DbIndex();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (schemaObject == null)
                throw new Exception("Unable to identify object");

            schemaObject.Definition = file.ReadAllText();
           (schemaObject.SchemaName , schemaObject.Name) =  SqlReader.ReadNames(objectType, schemaObject.Definition);

            return schemaObject;
        }
    }
}
