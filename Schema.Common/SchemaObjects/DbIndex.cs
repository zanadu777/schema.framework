using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schema.Common.SchemaObjects
{
   public class DbIndex: DbSchemaObject
    {
        public override ESchemaObjectType SchemaObjectType => ESchemaObjectType.Index;

        public string TableFullName { get; set; }
        public string IndexType { get; set; }
        public bool IsUnique { get; set; }
        public List<DbIndexColumn> Columns = new List<DbIndexColumn>();

        public string GenerateDefinition()
        {
            var cb = new CodeBuilder();
            cb.AppendLine($"Create {IndexType} INDEX {Name} ON {TableFullName}");
            cb.AppendLine("(");
            cb.Indent();
            cb.AppendLineDelimited(",", Columns, c => $"{c.Name} {Direction(c.IsDescending)}");   
            cb.Outdent();
            cb.AppendLine(")");
            return cb.ToString();
        }

        private static string Direction(bool isDescending)
        {
            return (isDescending) ? "DESC" : "ASC";
        }

    }
}
