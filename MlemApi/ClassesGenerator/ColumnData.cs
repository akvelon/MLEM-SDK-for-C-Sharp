namespace MlemApi.ClassesGenerator
{
    /// <summary>
    /// Mapping type between dataframe column name and result class field/type
    /// </summary>
    internal class ColumnData
    {
        public string NameInModel { get; set; }
        public string NameInClass { get; set; }
        public string TypeInClass { get; set; }
    }
}
