using MlemApi.Dto;
using MlemApi.Serializing;

namespace MlemApi
{
    /// <summary>
    /// Builds request json body for incoming requests
    /// </summary>
    public class RequestBuilder
    {
        private readonly IRequestValuesSerializer _requestValueSerializer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestValueSerializer">request value serilaizer - which serializes request params to the json body</param>
        public RequestBuilder(IRequestValuesSerializer requestValueSerializer)
        {
            _requestValueSerializer = requestValueSerializer;
        }

        /// <summary>
        /// Builds request from parameters - returns request json body
        /// </summary>
        /// <typeparam name="T">Type of input data element</typeparam>
        /// <param name="argsName">Argument name of the method in mlem model schema</param>
        /// <param name="values">List of input values</param>
        /// <param name="argsType">type of the arguments data (which exact class implementing IApiDescriptionDataStructure is used)</param>
        /// <returns></returns>
        public string BuildRequest<T>(string argsName, IEnumerable<T> values, Type argsType)
            => _requestValueSerializer.Serialize(values, argsName, argsType);
    }
}
