using System;
using System.IO;
using System.Collections.Generic;

namespace FC.Unpacker
{
    class LpkgUnpack
    {
        static List<LpkgEntry> m_EntryTable = new List<LpkgEntry>();

        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            LpkgHashList.iLoadProject();
            using (FileStream TLpkgStream = File.OpenRead(m_Archive))
            {
                var lpSrcTable = TLpkgStream.ReadBytes((Int32)TLpkgStream.Length);
                var lpDstTable = Zlib.iDecompress(lpSrcTable);

                using (var TTableReader = new MemoryStream(lpDstTable))
                {
                    var m_TableHeader = new LpkgHeader();

                    m_TableHeader.dwMagic = TTableReader.ReadUInt32();
                    m_TableHeader.dwVersion = TTableReader.ReadInt32();
                    m_TableHeader.dwTotalFiles = TTableReader.ReadInt32();
                    m_TableHeader.dwTotalLpkgs = TTableReader.ReadInt32();

                    if (m_TableHeader.dwMagic != 0x4341434C)
                    {
                        throw new Exception("[ERROR]: Invalid magic of LPKG index file!");
                    }

                    if (m_TableHeader.dwVersion != 120 && m_TableHeader.dwVersion != 130)
                    {
                        throw new Exception("[ERROR]: Invalid version of LPKG index file!");
                    }

                    m_EntryTable.Clear();
                    for (Int32 i = 0; i < m_TableHeader.dwTotalFiles; i++)
                    {
                        var m_Entry = new LpkgEntry();

                        if (m_TableHeader.dwVersion == 120)
                        {
                            m_Entry.dwNameHash = TTableReader.ReadUInt32();
                            m_Entry.dwPackageID = TTableReader.ReadInt32();
                            m_Entry.dwOffset = TTableReader.ReadInt64();
                            m_Entry.dwSize = TTableReader.ReadInt64();

                            m_EntryTable.Add(m_Entry);
                        }
                        else if (m_TableHeader.dwVersion == 130)
                        {
                            m_Entry.dwNameHash = TTableReader.ReadUInt32();
                            m_Entry.dwCrc = TTableReader.ReadUInt32();
                            m_Entry.dwPackageID = TTableReader.ReadInt64();
                            m_Entry.dwOffset = TTableReader.ReadInt64();
                            m_Entry.dwSize = TTableReader.ReadInt64();

                            m_EntryTable.Add(m_Entry);
                        }

                    }

                    TTableReader.Dispose();
                }

                TLpkgStream.Dispose();
            }

            foreach (var m_Entry in m_EntryTable)
            {
                String m_FileName = LpkgHashList.iGetNameFromHashList(m_Entry.dwNameHash);
                String m_FullPath = m_DstFolder + m_FileName.Replace("/", @"\");
                String m_ArchiveFile = Path.GetDirectoryName(m_Archive) + @"\" + String.Format("data.lpkg.{0}", m_Entry.dwPackageID);

                Utils.iSetInfo("[UNPACKING]: " + m_FileName);
                Utils.iCreateDirectory(m_FullPath);

                using (FileStream TArchiveStream = File.OpenRead(m_ArchiveFile))
                {
                    TArchiveStream.Seek(m_Entry.dwOffset, SeekOrigin.Begin);

                    var lpBuffer = TArchiveStream.ReadBytes((Int32)m_Entry.dwSize);

                    lpBuffer = Zlib.iDecompress(lpBuffer);

                    File.WriteAllBytes(m_FullPath, lpBuffer);

                    TArchiveStream.Dispose();
                }
            }
        }
    }
}
