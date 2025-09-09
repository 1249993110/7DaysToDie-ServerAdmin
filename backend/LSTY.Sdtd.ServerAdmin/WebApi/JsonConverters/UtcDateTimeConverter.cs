using Newtonsoft.Json;

namespace LSTY.Sdtd.ServerAdmin.WebApi.JsonConverters
{
    internal class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                value = value.ToUniversalTime();
            }

            writer.WriteValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
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
