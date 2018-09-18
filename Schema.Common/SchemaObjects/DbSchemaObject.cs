using System;

namespace Schema.Common.SchemaObjects
{
    [Serializable]
    public abstract class DbSchemaObject
    {
        public string Name { get; set; }

        public string SchemaName { get; set; }

        /// <summary>
        /// The SQl definition of the object
        /// </summary>
        public string Definition { get; set; }

        public abstract ESchemaObjectType SchemaObjectType { get; }
    }
}
