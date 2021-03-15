using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

using MarkMyWords.Shared;

namespace MarkMyWords.Shared
{
    public static class Utils
    {
        /// <summary>
        /// Serializes and deserializes an object to get rid of all references.
        /// </summary>
        /// <typeparam name="T">The type of the object being cloned.</typeparam>
        /// <param name="obj">The object to clone.</param>
        /// <returns>The cloned object.</returns>
        public static T DeepClone<T>(this T obj)
        {
            JsonSerializerOptions options = DefaultOptions();
            string jsonString = JsonSerializer.Serialize(obj, options);
            return (T)JsonSerializer.Deserialize<T>(jsonString, options);
        }

        public static JsonSerializerOptions DefaultOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new PointModelConverterWithTypeDiscriminator());
            return options;
        }
    }
}
