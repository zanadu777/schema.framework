using System;
using System.Management.Automation;
using Schema.Common.Connectivity;
using Schema.SqlServer;

namespace PsSchema
{
    [Cmdlet(VerbsCommon.Get, "Schema")]
    public class GetSchema : PSCmdlet
    {
        private const string SchemaObject = "SchemaObject";
        private const string Name = "Name";

        [Parameter]
        public string ConnectionString { get; set; }

        [Parameter]
        [ValidateSet("FullSchema", SchemaObject, Name, "Definition")]
        public string Output { get; set; } = "FullSchema";
        protected override void ProcessRecord()
        {
            var connectionInfo = new DatabaseConnectionInfo();
            connectionInfo.ConnectionString = ConnectionString;
            var generator = new SchemaGenerator();
            var schema = generator.GetSchema(connectionInfo);

            if (Output == "FullSchema")
                WriteObject( schema);
            else if (Output == SchemaObject)
                foreach (var dbSchemaObject  in schema)
                    WriteObject(dbSchemaObject);
            else if (Output == Name)
                foreach (var dbSchemaObject in schema)
                    WriteObject(dbSchemaObject.Name);
            else if (Output == "Definition")
                foreach (var dbSchemaObject in schema)
                    WriteObject(dbSchemaObject.Definition);

        } 
    }
}
