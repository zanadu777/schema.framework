using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Schema.Common.SchemaObjects;

namespace Schema.SqlServer
{
    public class SqlReader
    {
        private static readonly Dictionary<ESchemaObjectType, Regex> nameReaders = new Dictionary<ESchemaObjectType, Regex>();

        public SqlReader()
        {
            
        }
        static SqlReader()
        {
            var functionRegex = new Regex(@"create\s+function\s+\[?(?<Schema>\w+)\]?\.\[?(?<Name>.*?)\]?\(", RegexOptions.IgnoreCase);
            var sysObjectRegex = new Regex(@"create\s+(view|table)\s+\[?(?<Schema>\w+)\]?\.\[?(?<Name>.*?)\]?\s+", RegexOptions.IgnoreCase);
            nameReaders.Add(ESchemaObjectType.ForeignKey, new Regex(@"constraint\s+(?<Name>.*?)\s+foreign.*?references\s+(?<Schema>\w+)", RegexOptions.IgnoreCase));
            nameReaders.Add(ESchemaObjectType.Index, new Regex(@"index\s+(?<Name>.*?)\s+on\s+(?<Schema>\w+)", RegexOptions.IgnoreCase));
            nameReaders.Add(ESchemaObjectType.ScalarFunction, functionRegex);
            nameReaders.Add(ESchemaObjectType.StoredProcedure, new Regex(@"create\s+procedure\s+\[?(?<Schema>\w+)\]?\.\[?(?<Name>.*?)\]?\s+", RegexOptions.IgnoreCase));
            nameReaders.Add(ESchemaObjectType.Table, sysObjectRegex);
            nameReaders.Add(ESchemaObjectType.TableFunction, functionRegex);
            nameReaders.Add(ESchemaObjectType.Trigger, new Regex(@"create\s+trigger\s+\[?(?<Schema>\w+)\]?\.\[?(?<Name>.*?)\]?\s+", RegexOptions.IgnoreCase));
            nameReaders.Add(ESchemaObjectType.View, sysObjectRegex);
        }
        public static (string Schema, string Name) ReadNames(ESchemaObjectType objectType, string sql)
        {
            Match match = nameReaders[objectType].Match(sql);
            var schema = match.Groups["Schema"].Value;
            if (string.IsNullOrWhiteSpace(schema))
                throw new Exception("Missing Schema");

            var name = match.Groups["Name"].Value;
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Missing Name");
           
            return (schema, name);
        }
    }
}
