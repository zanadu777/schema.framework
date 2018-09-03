namespace Schema.Common.SchemaObjects
{
   public enum EKeyStatus
    {
        None,
        PrimaryKey,
        ForeignKey,
        PrimaryAndForeignKey,
        ReferencedPrimaryKey,
        ReferencedPrimaryAndForeignKey
    }
}
