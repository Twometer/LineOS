using System;
using System.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.Listing;
using LineOS.NTFS.IO;
using LineOS.NTFS.Model.Attributes;

namespace LineOS.NTFS.Cosmos
{
    public class NtfsDirectoryEntry : DirectoryEntry
    {
        public NtfsFileEntry NtfsEntry;

        public NtfsDirectoryEntry(FileSystem aFileSystem, DirectoryEntry aParent, string aFullPath, string aName, long aSize, DirectoryEntryTypeEnum aEntryType, NtfsFileEntry entry) : base(aFileSystem, aParent, aFullPath, aName, aSize, aEntryType)
        {
            NtfsEntry = entry;
        }

        public override void SetName(string aName)
        {
            
        }

        public override void SetSize(long aSize)
        {
            
        }

        public override Stream GetFileStream()
        {
            var frec = NtfsEntry.MFTRecord;
            foreach (var att in frec.Attributes)
            {
                switch (att)
                {
                    case AttributeGeneric gen:
                        return new MemoryStream(gen.Data);
                    case AttributeData data:
                        return new MemoryStream(data.DataBytes);
                }
            }
            throw new Exception("ntfs: data attribute not found");
        }

        public override long GetUsedSpace()
        {
            return 0;
        }
    }
}
