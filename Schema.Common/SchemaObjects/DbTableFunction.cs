using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schema.Common.SchemaObjects
{
    public class DbTableFunction: DbSchemaObject
    {
        public override ESchemaObjectType SchemaObjectType => ESchemaObjectType.TableFunction;
    }
}
