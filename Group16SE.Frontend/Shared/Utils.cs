using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

using Group16SE.Frontend.Shared;

namespace Group16SE.Frontend.Shared
{
    public static class Utils
    {
        public static T DeepClone<T>(this T obj)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());
            string jsonString = JsonSerializer.Serialize(obj, options);
            return (T)JsonSerializer.Deserialize<T>(jsonString, options);
        }
    }
}
