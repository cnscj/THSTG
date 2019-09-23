using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.GZip;
using System.IO;

namespace XLibrary.Network
{
    /// <summary>
    /// Zip util. 压缩方法的封装，主要是用了 ICSharpCode.SharpZipLib 。
    /// C#自带的两个压缩类(System.IO.Compression下面)，在安卓平台不支持，除非设置IL2CPP，因此引入了这个压缩库。
    /// </summary>
    public sealed class ZipUtil
    {
        /// <summary>
        /// Compresses data with format 'GZip'.
        /// </summary>
        public static byte[] Compress_GZip(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();
            using (GZipOutputStream compressZipStream = new GZipOutputStream(outputStream, 512))
            {
                compressZipStream.SetLevel(6);  // 本来 Deflater.DEFAULT_COMPRESSION 也会自动转为6，但是又设置为-1...
                compressZipStream.IsStreamOwner = true;

                compressZipStream.Write(data, 0, data.Length);
                //compressZipStream.Flush();
                //compressZipStream.Finish();  //这一行必须调用，否则可能字节还没写进去.
            }

            byte[] output = outputStream.ToArray();
            outputStream.Close();

            return output;
        }

        /// <summary>
        /// Decompresses data with format 'GZip'.
        /// </summary>
        public static byte[] Decompress_GZip(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();

            MemoryStream inputStream = new MemoryStream(data);
            using (GZipInputStream decompressZipStream = new GZipInputStream(inputStream))
            {
                decompressZipStream.IsStreamOwner = true;

                int count;
                byte[] buffer = new byte[128];
                while ((count = decompressZipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, count);
                }
            }

            byte[] output = outputStream.ToArray();
            outputStream.Close();

            return output;
        }

        /// <summary>
        /// Compresses data with format 'Deflate'. 对应 php.gzcompress
        /// 调用该函数记得加try...catch...
        /// </summary>
        public static byte[] Compress_Deflate(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();
            using (DeflaterOutputStream compressZipStream = new DeflaterOutputStream(outputStream))
            {
                compressZipStream.Write(data, 0, data.Length);
                compressZipStream.Flush();
                compressZipStream.Finish();  //这一行必须调用，否则可能字节还没写进去.
            }

            byte[] output = outputStream.ToArray();
            outputStream.Close();

            return output;
        }

        /// <summary>
        /// Decompresses data with format 'Deflate'. 对应 php.gzuncompress
        /// 调用该函数记得加try...catch...
        /// </summary>
        public static byte[] Decompress_Deflate(byte[] data)
        {
            MemoryStream outputStream = new MemoryStream();

            MemoryStream inputStream = new MemoryStream(data);
            using (InflaterInputStream decompressZipStream = new InflaterInputStream(inputStream))
            {
                int count;
                byte[] buffer = new byte[128];
                while ((count = decompressZipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, count);
                }
            }

            byte[] output = outputStream.ToArray();
            outputStream.Close();

            return output;
        }
    }
}
