namespace MlemApi.Utils
{
    internal class CamelCaseConverter
    {
        public string ConvertToCamelCase(string str)
        {
            return string.Concat(str.Split('-', '_', '\n', '.', ':', ' ')
                .Select(s => char.ToUpper(s[0]) + s.Substring(1)));
        }
    }
}
