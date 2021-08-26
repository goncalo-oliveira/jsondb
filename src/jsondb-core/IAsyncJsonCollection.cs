using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonDb
{
    public interface IAsyncJsonCollection<T> : IJsonCollection<T>
    {
        Task<IJsonCollection<T>> GetCollectionAsync();
    }
}
