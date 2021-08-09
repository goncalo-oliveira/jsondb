using System;

namespace JsonDb
{
    public interface IJsonDbFactory
    {
        IJsonDb GetJsonDb();
    }
}
