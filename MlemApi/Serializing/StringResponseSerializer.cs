using MlemApi.MessageResources;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace MlemApi.Serializing
{
    public class StringResponseSerializer : IResponseSerializer
    {
        public T Serialize<T>(HttpResponseMessage response)
        {
            var stringResponse = response.Content.ReadAsStringAsync().Result;

            try
            {
                T? result = JsonSerializer.Deserialize<T>(stringResponse);

                return result;
            }
            catch
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));

                return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, stringResponse);
            }
        }
    }
}
