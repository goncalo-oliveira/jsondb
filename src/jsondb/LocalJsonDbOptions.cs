using System;

namespace JsonDb.Local
{
    public class LocalJsonDbOptions : JsonDbOptions
    {
        public string DbPath { get; set; }
    }
}
