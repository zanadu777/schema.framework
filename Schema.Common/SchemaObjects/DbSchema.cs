using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Schema.Common.SchemaObjects
{
    public class DbSchema : IEnumerable<DbSchemaObject>
    {
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
