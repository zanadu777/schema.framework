namespace Schema.Common.SchemaObjects
{
    public class DbParameter
    {
        public string Name { get; set; }
        public int Ordinal { get; set; }
        public string DataType { get; set; }
        public int MaxLength { get; set; }

    }
}