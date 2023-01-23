using System.Text.Json;
using MlemApi.Dto;

namespace MlemApi.Parsing
{
    /// <summary>
    /// Main interface for specific data type parsers - see DataType as relevant mlem class - https://github.com/iterative/mlem/blob/663862f233c6422c7a27fef682eb8f1a3d4a46ea/mlem/core/data_type.py#L33
    /// </summary>
    internal interface IDataTypeProvider
    {
        /// <summary>
        /// Returns list of mlem aliases of supported types
        /// </summary>
        /// <returns></returns>
        List<string> GetSupportedTypes();
        /// <summary>
        /// Returns parsed type for data schema provided (content of 'data_type' field)
        /// </summary>
        /// <param name="argsObjectEnumerator">Json content of 'data_type' field in schema</param>
        /// <param name="childDataTypeProvider">Child data provider </param>
        /// <returns>Parsed type</returns>
        IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator argsObjectEnumerator, IDataTypeProvider childDataTypeProvider);
    }
}
