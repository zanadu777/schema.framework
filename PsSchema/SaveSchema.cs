using System;
using System.IO;
using System.Management.Automation;
using Errata.IO;
using Schema.Common.SchemaObjects;

namespace PsSchema
{
    [Cmdlet(VerbsData.Save, "Schema")]
    public class SaveSchema : PSCmdlet
    {
        [Parameter(Mandatory=true, ValueFromPipeline = true, Position=0)]
        public DbSchema Schema { get; set; }

        [Parameter]
        public string Location { get; set; }
        protected override void ProcessRecord()
        {
            var dir = new DirectoryInfo(Location);
            foreach (var schemaObject in Schema)
                dir.WriteAllText($"{schemaObject.SchemaObjectType}_{schemaObject.Name}.sql" , schemaObject.Definition);
        }
    }
}
