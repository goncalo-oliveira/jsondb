using System;
using System.Text.Json;

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
    }
}
