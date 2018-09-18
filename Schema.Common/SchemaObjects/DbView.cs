using System.Collections.ObjectModel;

namespace Schema.Common.SchemaObjects
{
    public class DbView : DbSchemaObject
    {
        public override ESchemaObjectType SchemaObjectType => ESchemaObjectType.View;
        public ObservableCollection<DbColumn> Columns { get; set; } = new ObservableCollection<DbColumn>();
    }
}
