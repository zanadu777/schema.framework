using System;
using System.IO;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using Schema.Common.Connectivity;
using Schema.Common.SchemaObjects;
using Schema.SqlServer;

namespace PsSchema
{
    [Cmdlet(VerbsCommon.Get, "Schema")]
    public class GetSchema : PSCmdlet
    {
        private const string SchemaObject = "SchemaObject";
        private const string Name = "Name";

        [Parameter]
        [Parameter(ParameterSetName = "fromDatabase")]
        public string ConnectionString { get; set; }

        [Parameter]
        [ValidateSet("FullSchema", SchemaObject, Name, "Definition")]
        public string Output { get; set; } = "FullSchema";

        [Parameter]
        [Parameter(ParameterSetName = "fromDirectory")]
        public string Directory { get; set; }

        protected override void ProcessRecord()
        {
            DbSchema schema = null;
            if (this.ParameterSetName == "fromDatabase")
            {
                var connectionInfo = new DatabaseConnectionInfo();
                connectionInfo.ConnectionString = ConnectionString;
                var generator = new SchemaGenerator();
                schema = generator.GetSchema(connectionInfo);
            }
            else if (this.ParameterSetName == "fromDirectory")
            {
                var generator = new SchemaFromFileSystem();
                schema = generator.GetSchem(new DirectoryInfo(Directory));
            }

            if (schema == null)
                return;

            if (Output == "FullSchema")
                WriteObject(schema, false);
            else if (Output == SchemaObject)
                foreach (var dbSchemaObject in schema)
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
