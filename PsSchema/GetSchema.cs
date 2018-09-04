using System;
using System.Management.Automation;
using Schema.Common.Connectivity;
using Schema.SqlServer;

namespace PsSchema
{
    [Cmdlet(VerbsCommon.Get, "Schema")]
    public class GetSchema : PSCmdlet
    {
        [Parameter]
        public string ConnectionString { get; set; }

        [Parameter]
        [ValidateSet("FullSchema", "SchemaObjects")]
        public string Output { get; set; } = "FullSchema";
        protected override void ProcessRecord()
        {
            var connectionInfo = new DatabaseConnectionInfo();
            connectionInfo.ConnectionString = ConnectionString;
            var generator = new SchemaGenerator();
            var schema = generator.GetSchema(connectionInfo);

            if (Output == "FullSchema")
                WriteObject( schema);
            else if (Output == "SchemaObjects")
                foreach (var dbSchemaObject  in schema)
                    WriteObject(schema);
        }
    }
}
