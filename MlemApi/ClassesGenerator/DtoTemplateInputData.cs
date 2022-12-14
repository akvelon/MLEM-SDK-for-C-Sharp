namespace MlemApi.ClassesGenerator
{
    /// <summary>
    /// Input data for class template (dataframe type)
    /// </summary>
    internal class DtoTemplateInputData
    {
        public string AccessModifier { get; set; }
        public string NamespaceName { get; set; }
        public string ClassName { get; set; }
        public List<ColumnData> ColumnsData { get; set; }
    }
}
