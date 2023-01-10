using System;
using System.IO;
using System.IO.Compression;

namespace FC.Unpacker
{
    class Zlib
    {
        public static Byte[] iDecompress(Byte[] lpBuffer)
        {
            var TOutMemoryStream = new MemoryStream();
            using (MemoryStream TMemoryStream = new MemoryStream(lpBuffer))
            {
                var m_Header = new LpkgCompressedHeader();

                m_Header.dwMagic = TMemoryStream.ReadUInt32();
                m_Header.dwDecompressedSize = TMemoryStream.ReadInt32();

                if (m_Header.dwMagic != 0x3144434C)
                {
                    return lpBuffer;
                }

                TMemoryStream.Position += 2;

                using (DeflateStream TDeflateStream = new DeflateStream(TMemoryStream, CompressionMode.Decompress, false))
                {
                    TDeflateStream.CopyTo(TOutMemoryStream);
                    TDeflateStream.Dispose();
                }
                TMemoryStream.Dispose();
            }

            return TOutMemoryStream.ToArray();
        }
    }
}