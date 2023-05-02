﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Route256.Week6.Homework.PriceCalculator.SerializationUtlis.NamingPolicies;

namespace Route256.Week5.Homework.PriceCalculator.BackgroundServices.Serializers;

public sealed class JsonValueSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public JsonValueSerializer()
    {
        _serializerOptions = new JsonSerializerOptions();
        _serializerOptions.Converters.Add(new JsonStringEnumConverter());
        _serializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    }

    public byte[] Serialize(T data, SerializationContext context) =>
        JsonSerializer.SerializeToUtf8Bytes(data, _serializerOptions);

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException(nameof(data), "Null data encountered");

        return JsonSerializer.Deserialize<T>(data, _serializerOptions) ??
               throw new ArgumentNullException(nameof(data), "Null data encountered");
    }
}