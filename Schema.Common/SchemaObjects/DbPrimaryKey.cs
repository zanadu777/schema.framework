using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schema.Common.SchemaObjects
{
  public  class DbPrimaryKey : DbSchemaObject
    {
        public override ESchemaObjectType SchemaObjectType => ESchemaObjectType.PrimaryKey;

        public String Table { get; set; }
        public string TableSchema { get; set; }
        public string FileGroup { get; set; }
        public int Partition { get; set; }
        public string Type { get; set; }

        public bool IsDisabled { get; set; }
        public bool IsPadded { get; set; }
        public bool IsAllowingPageLocks { get; set; }

        public bool IsAllowingRowLocks { get; set; }

        public long RowCount { get; set; }
    }
}
