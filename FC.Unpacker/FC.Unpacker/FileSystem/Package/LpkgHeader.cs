using System;

namespace FC.Unpacker
{
    class LpkgHeader
    {
        public UInt32 dwMagic { get; set; } // LCAC (0x4341434C)
        public Int32 dwVersion { get; set; } // 120 (Version???)
        public Int32 dwTotalFiles { get; set; }
        public Int32 dwTotalLpkgs { get; set; }
    }

    class LpkgCompressedHeader
    {
        public UInt32 dwMagic { get; set; } // LCD1 (0x3144434C)
        public Int32 dwDecompressedSize { get; set; }
    }
}
