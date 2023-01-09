using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace BibleTaggingUtil
{
    /// <summary>
    /// This class is based on the Sample factory pattern converter taken from 
    ///   https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-6-0
    ///   This class is modified to support serializing and deserializing a dictionary of type:Dictionary<string, Dictionary<int, string[]>>
    ///   
    /// </summary>
    internal class DictionaryJsonSirlizer : JsonConverterFactory
    {
 

        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            {
                return false;
            }

            Type[] t = typeToConvert.GetGenericArguments();
            //bool res = t[0].IsEnum;
            bool res = (t[0] == typeof(string));
            return res;
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            Type keyType = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(DictionaryEnumConverterInner<,>).MakeGenericType(
                    new Type[] { keyType, valueType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null)!;

            return converter;
        }

        private class DictionaryEnumConverterInner<TKey, TValue> :
            JsonConverter<Dictionary<TKey, TValue>> where TKey : notnull //struct, Enum
        {
            private readonly JsonConverter<TValue> _valueConverter;
            private readonly Type _keyType;
            private readonly Type _valueType;

            public DictionaryEnumConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter.
                _valueConverter = (JsonConverter<TValue>)options
                    .GetConverter(typeof(TValue));

                // Cache the key and value types.
                _keyType = typeof(TKey);
                _valueType = typeof(TValue);
            }

            public override Dictionary<TKey, TValue> Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var dictionary = new Dictionary<TKey, TValue>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dictionary;
                    }

                    // Get the key.
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string? propertyName = reader.GetString();
                    TKey key = (TKey)Convert.ChangeType(propertyName == null ? "" : propertyName, typeof(TKey));

                    // TODO
                    // For performance, parse with ignoreCase:false first.
                    //if (!Enum.TryParse(propertyName, ignoreCase: false, out TKey key) &&
                    //    !Enum.TryParse(propertyName, ignoreCase: true, out key))
                    //{
                    //    throw new JsonException(
                    //        $"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\".");
                    //}
                    // Get the value.
                    reader.Read();
                    TValue value = _valueConverter.Read(ref reader, _valueType, options)!;

                    // Add to dictionary.
                    dictionary.Add(key, value);
                }

                throw new JsonException();
            }

            public override void Write(
                Utf8JsonWriter writer,
                Dictionary<TKey, TValue> dictionary,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach ((TKey key, TValue value) in dictionary)
                {
                    var propertyName = key.ToString();
                    writer.WritePropertyName
                        (options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);

                    _valueConverter.Write(writer, value, options);
                }

                writer.WriteEndObject();
            }
        }
        }
}
