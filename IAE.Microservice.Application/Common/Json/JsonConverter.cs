﻿using Newtonsoft.Json;
using System;

namespace IAE.Microservice.Application.Common.Json
{
    public class JsonConverter<TInterface, TClass> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TInterface);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(TClass));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, typeof(TClass));
        }
    }
}