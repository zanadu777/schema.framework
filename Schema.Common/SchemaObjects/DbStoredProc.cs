using System.Collections.ObjectModel;

namespace Schema.Common.SchemaObjects
{
    public class DbStoredProc : DbSchemaObject
    {
        public override ESchemaObjectType SchemaObjectType => ESchemaObjectType.StoredProcedure;

        public ObservableCollection<DbParameter> Parameters { get; set; }=new ObservableCollection<DbParameter>();
    }
}
