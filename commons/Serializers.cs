using System;
using System.Globalization;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace vault.commons;


public class TimestampJsonConverter : JsonConverter<Timestamp>
{
    public override void WriteJson(JsonWriter writer, Timestamp value, JsonSerializer serializer) 
        => writer.WriteValue($"{value.ToDateTimeOffset():O}");

    public override Timestamp ReadJson(JsonReader reader, Type objectType, Timestamp existingValue, 
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader?.Value is not string s)
            return new Timestamp();
        return Timestamp.FromDateTimeOffset(DateTimeOffset.ParseExact(s, $"O", DateTimeFormatInfo.InvariantInfo));
    }
}
public class TimestampNullableJsonConverter : JsonConverter<Timestamp?>
{
    public override void WriteJson(JsonWriter writer, Timestamp? value, JsonSerializer serializer)
    {
        if (value is null)
            return;
        writer.WriteValue($"{value?.ToDateTimeOffset():O}");
    }

    public override Timestamp? ReadJson(JsonReader reader, Type objectType, Timestamp? existingValue, 
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader?.Value is not string s)
            return null;
        return Timestamp.FromDateTimeOffset(DateTimeOffset.ParseExact(s, $"O", DateTimeFormatInfo.InvariantInfo));
    }
}