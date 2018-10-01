using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Errata.Collections;

namespace Schema.Common.SchemaObjects
{
    //[DebuggerVisualizer(typeof(DbTableVisualizer))]
    [Serializable]
    public class DbTable : DbSchemaObject
    {
        public DbTable()
        {
            Columns = new ObservableCollection<DbColumn>();

        }

        public override ESchemaObjectType SchemaObjectType => ESchemaObjectType.Table;
        public ObservableCollection<DbColumn> Columns { get; set; }

        public DataTable ColumnDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("DataType", typeof(string)));
            dt.Columns.Add(new DataColumn("Ordinal", typeof(int)));
            dt.Columns.Add(new DataColumn("MaxLength", typeof(int)));
            dt.Columns.Add(new DataColumn("IsIdentity", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsNullable", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsInPrimaryKey", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsForeignKey", typeof(bool)));
            dt.Columns.Add(new DataColumn("IsReferencedPrimaryKey", typeof(bool)));
            dt.Columns.Add(new DataColumn("DisplayDataType", typeof(string)));

            foreach (var column in Columns)
            {
                var row = dt.NewRow();

                row["Name"] = column.Name;
                row["DataType"] = column.DataType;
                row["Ordinal"] = column.Ordinal;
                row["MaxLength"] = column.MaxLength;
                row["IsIdentity"] = column.IsIdentity;
                row["IsNullable"] = column.IsNullable;
                row["IsInPrimaryKey"] = column.IsInPrimaryKey;
                row["IsForeignKey"] = column.IsForeignKey;
                row["IsReferencedPrimaryKey"] = column.IsReferencedPrimaryKey;
                row["DisplayDataType"] = column.DisplayDataType;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public List<DbColumn> NonIdentityColumns => Columns.Where(c => !c.IsIdentity).ToList();

        public List<DbColumn> PrimaryKeyColumns => Columns.Where(c => c.IsInPrimaryKey).ToList();

        public List<DbForeignKey> ForeignKeyConstraints = new List<DbForeignKey>();
        public List<DbForeignKey> ForeignKeyReferences = new List<DbForeignKey>();

        public bool HasPrimayKey => Columns.Any(c => c.IsInPrimaryKey);

        public DbPrimaryKey PrimaryKey { get; set; } = new DbPrimaryKey();
        public long RowCount { get; set; }
        public string FileGroup { get; set; }
        public int Partition { get; set; }

        public string GenerateDefinition()
        {
            var rl = Columns.ToRemainderLast();
            CodeBuilder cb = new CodeBuilder();
            cb.IndentLength = 4;
            cb.AppendLine($"Create Table {SchemaName}.{Name}");
            cb.Indent();
            cb.AppendLine("(");
            foreach (var col in rl.Remainder)
                cb.AppendLine($"{col.Declaration},");

            cb.AppendLine($"{rl.Last.Declaration}{((HasPrimayKey) ? "," : "")}");

            if (HasPrimayKey)
            {
                cb.AppendLine($"CONSTRAINT {PrimaryKey.Name} PRIMARY KEY {PrimaryKey.Type}");
                cb.Indent();
                cb.StartLine("(").AppendDelimited(",", PrimaryKeyColumns, c=> c.Name).EndLine(")");
                cb.Outdent();
            }

            cb.Outdent();
            cb.AppendLine(")");
            return cb.ToString().Trim();
        }

    }
}
