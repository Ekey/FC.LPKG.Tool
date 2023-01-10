using System;

namespace FC.Unpacker
{
    class LpkgEntry
    {
        public UInt32 dwNameHash { get; set; }
        public Int32 dwPackageID { get; set; }
        public Int64 dwOffset { get; set; }
        public Int64 dwSize { get; set; }
    }
}
