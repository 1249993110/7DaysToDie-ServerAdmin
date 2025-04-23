using System.Text.Json.Serialization;

namespace LSTY.Sdtd.ServerAdmin.WebApi.JsonConverters
{
    /// <summary>
    /// Json converter for object to inferred types.
    /// </summary>
    public class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out long l))
                        return l;
                    return reader.GetDouble();
                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out DateTime datetime))
                        return datetime;
                    return reader.GetString()!;
                case JsonTokenType.StartObject:
                    using (var doc = JsonDocument.ParseValue(ref reader))
                    {
                        return ReadObject(doc.RootElement);
                    }
                case JsonTokenType.StartArray:
                    using (var doc = JsonDocument.ParseValue(ref reader))
                    {
                        return ReadArray(doc.RootElement);
                    }
                case JsonTokenType.Null:
                    return null!;
                default:
                    throw new JsonException($"Unsupported token type: {reader.TokenType}");
            }
        }

        private static Dictionary<string, object?> ReadObject(JsonElement element)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in element.EnumerateObject())
            {
                dict[prop.Name] = ConvertElement(prop.Value);
            }
            return dict;
        }

        private static List<object?> ReadArray(JsonElement element)
        {
            var list = new List<object?>();
            foreach (var item in element.EnumerateArray())
            {
                list.Add(ConvertElement(item));
            }
            return list;
        }

        private static object? ConvertElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => ReadObject(element),
                JsonValueKind.Array => ReadArray(element),
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out long l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => throw new JsonException($"Unsupported JsonValueKind: {element.ValueKind}")
            };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="objectToWrite"></param>
        /// <param name="options"></param>
        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
    }
}
