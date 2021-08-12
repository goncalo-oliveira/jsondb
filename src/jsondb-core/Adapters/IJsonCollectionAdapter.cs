using System;
using System.Threading.Tasks;

namespace JsonDb.Adapters
{
    /// <summary>
    /// An adapter for a collection data
    /// </summary>
    public interface IJsonCollectionAdapter
    {
        /// <summary>
        /// Reads input data through the adapter
        /// </summary>
        /// <param name="input">The input data</param>
        /// <returns>Input data</returns>
        Task<byte[]> ReadAsync( byte[] input );

        /// <summary>
        /// Writes output data through the adapter
        /// </summary>
        /// <param name="output">The output data</param>
        /// <returns>Output data</returns>
        Task<byte[]> WriteAsync( byte[] output );
    }
}
