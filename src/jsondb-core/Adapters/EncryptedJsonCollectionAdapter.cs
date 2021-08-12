using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace JsonDb.Adapters
{
    /// <summary>
    /// An adapter to encrypt/decrypt a collection with AES
    /// </summary>
    public sealed class EncryptedJsonCollectionAdapter : IJsonCollectionAdapter
    {
        private readonly byte[] key;

        public EncryptedJsonCollectionAdapter( byte[] encryptionKey )
        {
            key = encryptionKey;
        }

        /// <summary>
        /// Decrypts source collection data
        /// </summary>
        /// <param name="source">Source collection data to decrypt</param>
        /// <returns>Decrypted collection data</returns>
        public Task<byte[]> ReadAsync( byte[] source )
        {
            var iv = new byte[16];
            var cipher = new byte[source.Length - 16];

            Buffer.BlockCopy( source, 0, iv, 0, iv.Length );
            Buffer.BlockCopy( source, iv.Length, cipher, 0, cipher.Length );

            using ( var aesAlg = Aes.Create() )
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.KeySize = 256;

                using ( var decryptor = aesAlg.CreateDecryptor( key, iv ) )
                {
                    using ( var msResult = new MemoryStream() )
                    {
                        using ( var msDecrypt = new MemoryStream( cipher ) )
                        {
                            var buffer = new byte[8192];
                            using ( var cs = new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read ) )
                            {
                                int read;
                                while ( ( read = cs.Read( buffer, 0, buffer.Length ) ) > 0 )
                                {
                                    msResult.Write( buffer, 0, read );
                                }
                            }
                        }

                        msResult.Flush();

                        var decryptedData = msResult.ToArray();

                        return Task.FromResult( ArrayTrimEnd( decryptedData ) );
                    }
                }
                //
            }
            //
        }

        /// <summary>
        /// Encrypts collection data to write to storage
        /// </summary>
        /// <param name="target">Collection data to encrypt</param>
        /// <returns>Encrypted collection data</returns>
        public Task<byte[]> WriteAsync( byte[] target )
        {
            using ( var alg = Aes.Create() )
            {
                alg.Mode = CipherMode.CBC;
                alg.Padding = PaddingMode.Zeros;
                alg.KeySize = 256;

                using ( var encryptor = alg.CreateEncryptor( key, alg.IV ) )
                {
                    using ( var ms = new MemoryStream() )
                    {
                        using ( var cs = new CryptoStream( ms, encryptor, CryptoStreamMode.Write ) )
                        {
                            cs.Write( target, 0, target.Length );
                        }

                        var iv = alg.IV;
                        var decryptedContent = ms.ToArray();

                        var encryptedContent = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy( iv, 0, encryptedContent, 0, iv.Length );
                        Buffer.BlockCopy( decryptedContent, 0, encryptedContent, iv.Length, decryptedContent.Length );

                        return Task.FromResult( encryptedContent );
                    }
                }
                //
            }
            //
        }

        private byte[] ArrayTrimEnd( byte[] array )
        {
            int lastIndex = Array.FindLastIndex( array, b => b != 0 );

            Array.Resize( ref array, lastIndex + 1 );

            return ( array );
        }

    }
}
