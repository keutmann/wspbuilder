/* Program : WSPTools.BaseLibrary
 * Created by: Carsten Keutmann
 * Date : 2009
 *  
 * The WSPTools.BaseLibrary comes under GNU GENERAL PUBLIC LICENSE (GPL).
 */
using System;
using System.IO;
using System.IO.Compression;

namespace WSPTools.BaseLibrary.IO.Compression
{
    public class GZipLib
    {
        public static byte[] Compress(byte[] uncompressedBuffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzip.Write(uncompressedBuffer, 0, uncompressedBuffer.Length);
                }
                byte[] compressedBuffer = ms.ToArray();
                return compressedBuffer;
            }
        }

        public static byte[] Decompress(byte[] compressedBuffer)
        {
            using (GZipStream gzip = new GZipStream(new MemoryStream(compressedBuffer), CompressionMode.Decompress))
            {
                byte[] uncompressedBuffer = ReadAllBytes(gzip);
                return uncompressedBuffer;
            }
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            byte[] buffer = new byte[4096];
            using (MemoryStream ms = new MemoryStream())
            {
                int bytesRead = 0;
                do
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }
                } while (bytesRead > 0);
                return ms.ToArray();
            }
        }
    }
}
