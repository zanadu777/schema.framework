﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schema.Common.SchemaObjects
{
    public class DbTrigger:DbSchemaObject
    {
        public override ESchemaObjectType SchemaObjectType => ESchemaObjectType.Trigger;
    }
}
