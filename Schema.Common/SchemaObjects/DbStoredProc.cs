using System.Collections.ObjectModel;

namespace Schema.Common.SchemaObjects
{
    public class DbStoredProc : DbSchemaObject
    {
        public DbStoredProc()
        {
            SchemaObjectType = ESchemaObjectType.StoredProcedure;
            Parameters = new ObservableCollection<DbParameter>();
        }



        public ObservableCollection<DbParameter> Parameters { get; set; }
    }
}
