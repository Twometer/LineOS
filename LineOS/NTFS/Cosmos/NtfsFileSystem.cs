using System;
using System.Collections.Generic;
using Cosmos.HAL.BlockDevice;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.Listing;
using LineOS.FS;
using LineOS.NTFS.IO;

namespace LineOS.NTFS.Cosmos
{
    public class NtfsFileSystem : FileSystem
    {
        private readonly Partition device;
        private readonly string rootPath;
        private readonly long size;

        private Ntfs ntfs;

        public NtfsFileSystem(Partition aDevice, string aRootPath, long aSize) : base(aDevice, aRootPath, aSize)
        {
            device = aDevice;
            rootPath = aRootPath;
            size = aSize;
            Initialize();
        }

        private void Initialize()
        {
            Console.WriteLine("[NTFSDRV2] Initializing NTFS file system on drive " + rootPath);
            ntfs = Ntfs.Create(new BlockDeviceStream(device, size));
        }

        public override void DisplayFileSystemInfo()
        {
        }

        public override List<DirectoryEntry> GetDirectoryListing(DirectoryEntry baseDirectory)
        {
            if (!(baseDirectory is NtfsDirectoryEntry ntfsEntry)) throw new Exception("ntfs: invalid dirlist request");
            if (!(ntfsEntry.NtfsEntry is NtfsDirectory ntfsDir)) throw new Exception("ntfs: dirlist: not a directory");
            var result = new List<DirectoryEntry>();
            foreach (var f in ntfsDir.ListFiles())
            {
                result.Add(new NtfsDirectoryEntry(this, null, baseDirectory.mFullPath + "\\" + f.Name, f.Name,
                        0,
                        f is NtfsFile ? DirectoryEntryTypeEnum.File : DirectoryEntryTypeEnum.Directory, f));
            }

            return result;
        }

        public override DirectoryEntry GetRootDirectory()
        {
            return new NtfsDirectoryEntry(this, null, "\\", rootPath, size, DirectoryEntryTypeEnum.Directory, ntfs.GetRootDirectory());
        }

        public override DirectoryEntry CreateDirectory(DirectoryEntry aParentDirectory, string aNewDirectory)
        {
            return null;
        }

        public override DirectoryEntry CreateFile(DirectoryEntry aParentDirectory, string aNewFile)
        {
            return null;
        }

        public override void DeleteDirectory(DirectoryEntry aPath)
        {

        }

        public override void DeleteFile(DirectoryEntry aPath)
        {

        }

        public override void Format(string aDriveFormat, bool aQuick)
        {

        }

        public override long AvailableFreeSpace { get; }
        public override long TotalFreeSpace => size;
        public override string Type => "NTFS";
        public override string Label { get; set; }
    }
}
