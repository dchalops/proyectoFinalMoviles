using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace proyectoFinalMoviles.Models
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string CustomDateFormat = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Intenta convertir la fecha al formato esperado
            if (DateTime.TryParseExact(reader.GetString(), CustomDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            throw new JsonException($"Invalid DateTime format: {reader.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CustomDateFormat));
        }
    }
}
