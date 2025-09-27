using Newtonsoft.Json;

namespace ApiServer.Shared.Common
{
    /// <summary>
    /// Int64 到 String 转换器
    /// </summary>
    public class Int64ToStringConverter : JsonConverter<long>
    {
        public override void WriteJson(JsonWriter writer, long value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override long ReadJson(JsonReader reader, Type objectType, long existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();
            return long.TryParse(value, out var result) ? result : 0;
        }
    }

    /// <summary>
    /// 可空 Int64 到 String 转换器
    /// </summary>
    public class NullableInt64ToStringConverter : JsonConverter<long?>
    {
        public override void WriteJson(JsonWriter writer, long? value, JsonSerializer serializer)
        {
            if (value.HasValue)
            {
                writer.WriteValue(value.Value.ToString());
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override long? ReadJson(JsonReader reader, Type objectType, long? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value?.ToString();
            return long.TryParse(value, out var result) ? result : null;
        }
    }
}