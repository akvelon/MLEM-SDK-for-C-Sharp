namespace ResourceGenerator.Models
{
    /// <summary>
    /// Represents json resource file constant data for generation
    /// </summary>
    internal class JsonResourceFileData
    {
        public string FileName { get; set; }
        public string ClassName { get; set; }
        public string NetCommentBlock { get; set; }
        public string JavaCommentBlock { get; set; }
    }
}
