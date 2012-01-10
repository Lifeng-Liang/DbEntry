namespace Leafing.Data.Common
{
    public enum FieldType
    {
        Normal,
        HasOne,
        HasMany,
        BelongsTo,
        HasAndBelongsToMany,
        LazyLoad
    }

    public enum ColumnFunction
    {
        None,
        ToLower,
        ToUpper,
    }
}
