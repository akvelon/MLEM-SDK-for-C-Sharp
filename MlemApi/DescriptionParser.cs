using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MlemApi.Dto;

namespace MlemApi
{
    internal static class DescriptionParser
    {
        public static ApiDescription GetApiDescription(string jsonStringDescription)
        { 
            var jsonDescription = JObject.Parse(jsonStringDescription);
            var jsonDescriptionMethods = (JToken)jsonDescription["methods"];

            var description = new ApiDescription 
            { 
                Methods = new List<MethodDescription>(jsonDescriptionMethods.Count()) 
            };
            
            foreach (JProperty method in jsonDescriptionMethods)
            {
                description.Methods.Add(new MethodDescription 
                { 
                    MethodName = method.Name, 
                    ArgsName = method.Value["args"].First()["name"].ToString() 
                });
            };

            return description;
        }
    }
}
