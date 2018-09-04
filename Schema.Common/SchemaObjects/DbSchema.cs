using System;
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
            {
                switch (dbObject.SchemaObjectType)
                {
                    case ESchemaObjectType.Table:
                        Tables.Add((DbTable)dbObject);
                        break;
                    case ESchemaObjectType.View:
                        Views.Add((DbView)dbObject);
                        break;
                    case ESchemaObjectType.StoredProcedure:
                        StoredProcs.Add((DbStoredProc)dbObject);
                        break;
                    case ESchemaObjectType.ScalarFunction:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ObservableCollection<DbTable> Tables { get; set; } = new ObservableCollection<DbTable>();
        public ObservableCollection<DbView> Views { get; set; } = new ObservableCollection<DbView>();
        public ObservableCollection<DbStoredProc> StoredProcs { get; set; } = new ObservableCollection<DbStoredProc>();
        public IEnumerator<DbSchemaObject> GetEnumerator()
        {
            foreach (var item in Tables)
                yield return item;

            foreach (var item in Views)
                yield return item;

            foreach (var item in StoredProcs)
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
        }
    }
}
