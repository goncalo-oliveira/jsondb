using System;
using System.Text.Json;
using JsonDb.Adapters;

namespace JsonDb
{
    public class JsonDbOptions
    {
        public JsonDbOptions()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public JsonSerializerOptions JsonSerializerOptions { get; set; }

        public IJsonCollectionAdapter CollectionAdapter { get; set; }
    }
}
