using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace hb29.API.Helpers
{


    //public class JsonCustomDateConverter : JsonConverter<DateTime>
    //{
    //    private readonly string Format;
    //    public JsonCustomDateConverter(string format)
    //    {
    //        Format = format;
    //    }

    //    //public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    //    //{
    //    //    return DateTime.ParseExact(reader.ReadAsString(), Format, null);
    //    //}

    //    //public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    //    //{
    //    //    writer.WriteValue(value.ToString(Format));
    //    //}
    //    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    //    {
    //        writer.WriteStringValue(date.ToString(Format));
    //    }
    //    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        return DateTime.ParseExact(reader.GetString(), Format, null);
    //    }
    //}
}
