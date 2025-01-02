using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Celeste.Mod.Shockeline;

public class CustomJsonStringEnumConverter : JsonConverterFactory
{
    private static readonly JsonStringEnumConverter JsonStringEnumConverter = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return !typeToConvert.IsDefined(typeof(EnumAsIntegerAttribute), false) &&
               JsonStringEnumConverter.CanConvert(typeToConvert);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonStringEnumConverter.CreateConverter(typeToConvert, options);
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumAsIntegerAttribute : Attribute
    {
    }
}