using System.IO;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.Listing;
using LineOS.NTFS.IO;

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
            return null;
        }

        public override long GetUsedSpace()
        {
            return 0;
        }
    }
}
