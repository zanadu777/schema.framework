using System;
using System.Management.Automation;
using Errata.Collections;
using Errata.Collections.DataTypes;
using Schema.Common.SchemaObjects;

namespace PsSchema
{
    [Cmdlet(VerbsData.Compare, "Schema")]
    public class CompareSchema : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public DbSchema SchemaA { get; set; }


        [Parameter ]
        public DbSchema SchemaB { get; set; }


        protected override void ProcessRecord()
        {
            var tableCompare = SchemaA.Tables.CompareTo(SchemaB.Tables, t => t.UniqueName, (a, b) => a.Definition == b.Definition);
            WriteDifferenceSummary(tableCompare);

            var viewCompare = SchemaA.Views.CompareTo(SchemaB.Views, t => t.UniqueName, (a, b) => a.Definition == b.Definition);
            WriteDifferenceSummary(viewCompare);


            var storedProcCompare = SchemaA.StoredProcs.CompareTo(SchemaB.StoredProcs, t => t.UniqueName, (a, b) => a.Definition == b.Definition);
            WriteDifferenceSummary(storedProcCompare);

            var functionComapre = SchemaA.ScalarFunctions.CompareTo(SchemaB.ScalarFunctions, t => t.UniqueName, (a, b) => a.Definition == b.Definition);
            WriteDifferenceSummary(functionComapre);

            var tableFunctionComapre = SchemaA.TableFunctions.CompareTo(SchemaB.TableFunctions, t => t.UniqueName, (a, b) => a.Definition == b.Definition);
            WriteDifferenceSummary(tableFunctionComapre);

            var triggerCompare  = SchemaA.TableFunctions.CompareTo(SchemaB.TableFunctions, t => t.UniqueName, (a, b) => a.Definition == b.Definition);
            WriteDifferenceSummary(triggerCompare);
        }


        private void WriteDifferenceSummary<T>(KeyedComparisonResult<T, string> comparison) where T : DbSchemaObject
        {
            foreach (var item in comparison.OnlyA)
                WriteObject($"{item.SchemaObjectType} Only in SchemaA {item.UniqueName}");

            foreach (var item in comparison.OnlyB)
                WriteObject($"{item.SchemaObjectType} Only in SchemaB {item.UniqueName}");

            foreach (var item in comparison.Different)
                WriteObject($"{item.Key.SchemaObjectType} Different Between Schemas {item.Key.UniqueName}");
        }
    }
}
