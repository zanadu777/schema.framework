using System.Collections.ObjectModel;

namespace Schema.Common.SchemaObjects
{
   public class DbView : DbSchemaObject
    {
        public DbView()
        {
            SchemaObjectType = ESchemaObjectType.View;
            Columns = new ObservableCollection<DbColumn>();
        }
        public ObservableCollection<DbColumn> Columns { get; set; }
    }
}
