using Newtonsoft.Json;

namespace eCommerce.Infrastructure.Common.Extensions;

public class NoEscapeStringWriter : JsonConverter

{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().Replace("\\/", "/"));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return reader.Value.ToString();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(string);
    }

}
