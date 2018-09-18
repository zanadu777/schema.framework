using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schema.Common.Connectivity;
using Schema.Common.SchemaObjects;
using DbColumn = System.Data.Common.DbColumn;
using DbParameter = System.Data.Common.DbParameter;

namespace Schema.Common.Interfaces
{
    public interface ISchemaGenerator 
    {
        DbSchema GetSchema(DatabaseConnectionInfo connectionInfo);
        Dictionary<string, DbTable> GetTables(DatabaseConnectionInfo connectionInfo);

        Dictionary<string, DbView> GetViews(DatabaseConnectionInfo connectionInfo);
        Dictionary<string, DbView> GetViews(DatabaseConnectionInfo connectionInfo, string name);

        Dictionary<string, List<SchemaObjects.DbColumn>> GetViewColumns(DatabaseConnectionInfo connectionInfo, string name);
        Dictionary<string, List<SchemaObjects.DbColumn>> GetViewColumns(DatabaseConnectionInfo connectionInfo);

        Dictionary<string, DbStoredProc> GetStoredProcedures(DatabaseConnectionInfo connectionInfo);
        DbStoredProc GetStoredProcedure(DatabaseConnectionInfo connectionInfo, string name);

        Dictionary<string, List<SchemaObjects.DbParameter>> GetStoredProcedureParameters(DatabaseConnectionInfo connectionInfo);
        Dictionary<string, List<SchemaObjects.DbParameter>> GetStoredProcedureParameters(DatabaseConnectionInfo connectionInfo, string name);
    }
}
