using System;
using System.Text;

namespace Schema.Common.SchemaObjects
{
    [Serializable]
    public class DbColumn
    {
        public string Name { get; set; }

        public string ParamName => "@" + Name;
        public string DataType { get; set; }
        public int Ordinal { get; set; }

        public int MaxLength { get; set; }

        public bool IsIdentity { get; set; }

        public int IdentityStep { get; set; }
        public int IdentitySeed { get; set; }

        public bool IsNullable { get; set; }

        public bool IsInPrimaryKey { get; set; }

        public bool IsForeignKey { get; set; }

        public bool IsReferencedPrimaryKey { get; set; }

        public int Precision { get; set; }
        public int Scale { get; set; }

        public DbColumn() { }

        public string DisplayDataType
        {
            get
            {
                var dbDataType = DataType;
                switch (dbDataType)
                {
                    case "nvarchar":
                    case "nchar":
                        if (MaxLength == -1)
                            dbDataType += "(MAX)";
                        else
                            dbDataType += "(" + MaxLength / 2 + ")";
                        break;
                    case "decimal":
                    case "numeric":
                        dbDataType += $"({Precision}, {Scale})";
                        break;
                    case "varchar":
                    case "char":
                    case "varbinary":
                        if (MaxLength == -1)
                            dbDataType += "(MAX)";
                        else
                            dbDataType += "(" + MaxLength + ")";
                        break;
                }


                return dbDataType;
            }
        }

        public string Declaration
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Name);
                sb.Append(" ");
                sb.Append(DisplayDataType);
                if (IsIdentity)
                {
                    sb.Append($" IDENTITY({IdentitySeed}, {IdentityStep})");
                }
                if (!IsNullable)
                    sb.Append(" NOT NULL");


                return sb.ToString();
            }


        }


        public string ParameterDeclaration => $@"@{Name} {DisplayDataType}";


        public EKeyStatus KeyStatus
        {
            get
            {
                if (IsInPrimaryKey && IsForeignKey && IsReferencedPrimaryKey)
                    return EKeyStatus.ReferencedPrimaryAndForeignKey;

                if (IsInPrimaryKey && IsForeignKey)
                    return EKeyStatus.PrimaryAndForeignKey;

                if (IsInPrimaryKey && IsReferencedPrimaryKey)
                    return EKeyStatus.ReferencedPrimaryKey;

                if (IsInPrimaryKey)
                    return EKeyStatus.PrimaryKey;

                if (IsForeignKey)
                    return EKeyStatus.ForeignKey;

                return EKeyStatus.None;
            }
        }
    }
}
