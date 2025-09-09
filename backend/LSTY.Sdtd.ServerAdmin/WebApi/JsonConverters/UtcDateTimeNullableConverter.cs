using Newtonsoft.Json;

namespace LSTY.Sdtd.ServerAdmin.WebApi.JsonConverters
{
    internal class UtcDateTimeNullableConverter : JsonConverter<DateTime?>
    {
        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {
            if (value.HasValue == false)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }

        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.Value is string str)
            {
                if (DateTime.TryParse(str, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsedDate))
                {
                    return parsedDate;
                }
            }

            throw new JsonSerializationException($"Unable to convert the value '{reader.Value}' to a valid DateTime object.");
        }
    }
}
