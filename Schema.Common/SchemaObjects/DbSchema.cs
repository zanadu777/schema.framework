using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Schema.Common.SchemaObjects
{
    public class DbSchema : IEnumerable<DbSchemaObject>
    {
        public DbSchema()
        {

        }

        public DbSchema(IEnumerable<DbSchemaObject> dbObjects)
        {
            foreach (var dbObject in dbObjects)
                Add(dbObject);
        }
        public ObservableCollection<DbScalarFunction> ScalarFunctions { get; set; } = new ObservableCollection<DbScalarFunction>();
        public ObservableCollection<DbTableFunction> TableFunctions { get; set; } = new ObservableCollection<DbTableFunction>();

        public ObservableCollection<DbTrigger> Triggers { get; set; } = new ObservableCollection<DbTrigger>();
        public ObservableCollection<DbTable> Tables { get; set; } = new ObservableCollection<DbTable>();
        public ObservableCollection<DbView> Views { get; set; } = new ObservableCollection<DbView>();
        public ObservableCollection<DbStoredProc> StoredProcs { get; set; } = new ObservableCollection<DbStoredProc>();

        public ObservableCollection<DbForeignKey> ForeignKeys { get; set; } = new ObservableCollection<DbForeignKey>();
        public IEnumerator<DbSchemaObject> GetEnumerator()
        {
            foreach (var item in Tables)
                yield return item;

            foreach (var item in Views)
                yield return item;

            foreach (var item in StoredProcs)
                yield return item;

            foreach (var item in ScalarFunctions)
                yield return item;

            foreach (var item in TableFunctions)
                yield return item;

            foreach (var item in Triggers)
                yield return item;

            foreach (var item in ForeignKeys)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in Tables)
                yield return item;

            foreach (var item in Views)
                yield return item;

            foreach (var item in StoredProcs)
                yield return item;

            foreach (var item in ScalarFunctions)
                yield return item;

            foreach (var item in TableFunctions)
                yield return item;

            foreach (var item in Triggers)
                yield return item;

            foreach (var item in ForeignKeys)
                yield return item;
        }

        public void Add(DbSchemaObject schemaObject)
        {
            switch (schemaObject.SchemaObjectType)
            {
                case ESchemaObjectType.Table:
                    Tables.Add((DbTable)schemaObject);
                    break;
                case ESchemaObjectType.View:
                    Views.Add((DbView)schemaObject);
                    break;
                case ESchemaObjectType.StoredProcedure:
                    StoredProcs.Add((DbStoredProc)schemaObject);
                    break;
                case ESchemaObjectType.ScalarFunction:
                    ScalarFunctions.Add((DbScalarFunction)schemaObject);
                    break;
                case ESchemaObjectType.TableFunction:
                    TableFunctions.Add((DbTableFunction)schemaObject);
                    break;
                case ESchemaObjectType.Trigger:
                    Triggers.Add((DbTrigger)schemaObject);
                    break;
                case ESchemaObjectType.ForeignKey:
                    ForeignKeys.Add((DbForeignKey)schemaObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
