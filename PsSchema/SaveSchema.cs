using System;
using System.Management.Automation;
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

        }
    }
}
