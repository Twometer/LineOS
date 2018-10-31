using System;
using System.Collections.Generic;
using System.IO;
using Cosmos.HAL.BlockDevice;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using LineOS.CLI;
using LineOS.NTFS.Cosmos;

namespace LineOS.FS
{
    public class NtfsAwareCosmosVfs : VFSBase
    {
        private List<Partition> mPartitions;
        private List<FileSystem> mFileSystems;
        private FileSystem mCurrentFileSystem;
        private List<FileSystemFactory> mRegisteredFileSystems;
        private TablePrinter tablePrinter;

        /// <summary>
        /// Initializes the virtual file system.
        /// </summary>
        public override void Initialize()
        {
            mPartitions = new List<Partition>();
            mFileSystems = new List<FileSystem>();
            mRegisteredFileSystems = new List<FileSystemFactory>();
            tablePrinter = new TablePrinter();

            RegisterFileSystem(new NtfsFileSystemFactory());
            RegisterFileSystem(new FatFileSystemFactory());

            InitializePartitions();
            if (mPartitions.Count > 0)
            {
                InitializeFileSystems();
            }
        }

        public override void RegisterFileSystem(FileSystemFactory aFileSystemFactory)
        {
            mRegisteredFileSystems.Add(aFileSystemFactory);
        }

        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="aPath">The full path including the file to create.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">aPath</exception>
        public override DirectoryEntry CreateFile(string aPath)
        {

            if (aPath == null)
            {
                throw new ArgumentNullException(nameof(aPath));
            }

            if (aPath.Length == 0)
            {
                throw new ArgumentException("aPath");
            }


            if (File.Exists(aPath))
            {
                return GetFile(aPath);
            }

            string xFileToCreate = Path.GetFileName(aPath);

            string xParentDirectory = Path.GetDirectoryName(aPath);

            DirectoryEntry xParentEntry = GetDirectory(xParentDirectory);
            if (xParentEntry == null)
            {
                xParentEntry = CreateDirectory(xParentDirectory);
            }

            var xFS = GetFileSystemFromPath(xParentDirectory);
            return xFS.CreateFile(xParentEntry, xFileToCreate);
        }

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="aPath">The full path including the directory to create.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">aPath</exception>
        public override DirectoryEntry CreateDirectory(string aPath)
        {

            if (aPath == null)
            {
                throw new ArgumentNullException(nameof(aPath));
            }

            if (aPath.Length == 0)
            {
                throw new ArgumentException("aPath");
            }


            if (Directory.Exists(aPath))
            {
                return GetDirectory(aPath);
            }


            aPath = aPath.TrimEnd(DirectorySeparatorChar, AltDirectorySeparatorChar);

            string xDirectoryToCreate = Path.GetFileName(aPath);

            string xParentDirectory = Path.GetDirectoryName(aPath);

            DirectoryEntry xParentEntry = GetDirectory(xParentDirectory);

            if (xParentEntry == null)
            {
                xParentEntry = CreateDirectory(xParentDirectory);
            }


            var xFS = GetFileSystemFromPath(xParentDirectory);
            return xFS.CreateDirectory(xParentEntry, xDirectoryToCreate);
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="aPath">The full path.</param>
        /// <returns></returns>
        public override bool DeleteFile(DirectoryEntry aPath)
        {
            try
            {
                var xFS = GetFileSystemFromPath(aPath.mFullPath);
                xFS.DeleteFile(aPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes an empty directory.
        /// </summary>
        /// <param name="aPath">The full path.</param>
        /// <returns></returns>
        public override bool DeleteDirectory(DirectoryEntry aPath)
        {
            try
            {
                if (GetDirectoryListing(aPath).Count > 0)
                {
                    throw new Exception("Directory is not empty");
                }

                var xFS = GetFileSystemFromPath(aPath.mFullPath);
                xFS.DeleteDirectory(aPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the directory listing for a path.
        /// </summary>
        /// <param name="aPath">The full path.</param>
        /// <returns></returns>
        public override List<DirectoryEntry> GetDirectoryListing(string aPath)
        {
            var xFS = GetFileSystemFromPath(aPath);
            var xDirectory = DoGetDirectoryEntry(aPath, xFS);
            return xFS.GetDirectoryListing(xDirectory);
        }

        /// <summary>
        /// Gets the directory listing for a directory entry.
        /// </summary>
        /// <param name="aDirectory">The directory entry.</param>
        /// <returns></returns>
        public override List<DirectoryEntry> GetDirectoryListing(DirectoryEntry aDirectory)
        {
            if (aDirectory == null || String.IsNullOrEmpty(aDirectory.mFullPath))
            {
                throw new ArgumentException("Argument is null or empty", nameof(aDirectory));
            }

            return GetDirectoryListing(aDirectory.mFullPath);
        }

        /// <summary>
        /// Gets the directory entry for a directory.
        /// </summary>
        /// <param name="aPath">The full path path.</param>
        /// <returns>A directory entry for the directory.</returns>
        /// <exception cref="Exception"></exception>
        public override DirectoryEntry GetDirectory(string aPath)
        {
            try
            {
                var xFileSystem = GetFileSystemFromPath(aPath);
                var xEntry = DoGetDirectoryEntry(aPath, xFileSystem);
                if ((xEntry != null) && (xEntry.mEntryType == DirectoryEntryTypeEnum.Directory))
                {
                    return xEntry;
                }
            }
            catch (Exception)
            {
                return null;
            }
            throw new Exception(aPath + " was found, but is not a directory.");
        }

        /// <summary>
        /// Gets the directory entry for a file.
        /// </summary>
        /// <param name="aPath">The full path.</param>
        /// <returns>A directory entry for the file.</returns>
        /// <exception cref="Exception"></exception>
        public override DirectoryEntry GetFile(string aPath)
        {
            try
            {
                var xFileSystem = GetFileSystemFromPath(aPath);
                var xEntry = DoGetDirectoryEntry(aPath, xFileSystem);
                if ((xEntry != null) && (xEntry.mEntryType == DirectoryEntryTypeEnum.File))
                {
                    return xEntry;
                }
            }
            catch (Exception)
            {
                return null;
            }
            throw new Exception(aPath + " was found, but is not a file.");
        }

        /// <summary>
        /// Gets the volumes for all registered file systems.
        /// </summary>
        /// <returns>A list of directory entries for all volumes.</returns>
        public override List<DirectoryEntry> GetVolumes()
        {
            List<DirectoryEntry> xVolumes = new List<DirectoryEntry>();

            for (int i = 0; i < mFileSystems.Count; i++)
            {
                xVolumes.Add(GetVolume(mFileSystems[i]));
            }

            return xVolumes;
        }

        /// <summary>
        /// Gets the directory entry for a volume.
        /// </summary>
        /// <param name="aPath">The volume root path.</param>
        /// <returns>A directory entry for the volume.</returns>
        public override DirectoryEntry GetVolume(string aPath)
        {
            if (string.IsNullOrEmpty(aPath))
            {
                return null;
            }

            var xFileSystem = GetFileSystemFromPath(aPath);
            if (xFileSystem != null)
            {
                return GetVolume(xFileSystem);
            }

            return null;
        }

        /// <summary>
        /// Gets the attributes for a File / Directory.
        /// </summary>
        /// <param name="aPath">The path of the File / Directory.</param>
        /// <returns>The File / Directory attributes.</returns>
        public override FileAttributes GetFileAttributes(string aPath)
        {
            /*
             * We are limiting ourselves to the simpler attributes File and Directory for now.
             * I think that in the end FAT does not support anything else
             */

            var xFileSystem = GetFileSystemFromPath(aPath);
            var xEntry = DoGetDirectoryEntry(aPath, xFileSystem);

            if (xEntry == null)
                throw new Exception($"{aPath} is neither a file neither a directory");

            switch (xEntry.mEntryType)
            {
                case DirectoryEntryTypeEnum.File:
                    return FileAttributes.Normal;

                case DirectoryEntryTypeEnum.Directory:
                    return FileAttributes.Directory;

                case DirectoryEntryTypeEnum.Unknown:
                default:
                    throw new Exception($"{aPath} is neither a file neither a directory");
            }
        }

        /// <summary>
        /// Sets the attributes for a File / Directory.
        /// </summary>
        /// <param name="aPath">The path of the File / Directory.</param>
        /// <param name="fileAttributes">The attributes of the File / Directory.</param>
        public override void SetFileAttributes(string aPath, FileAttributes fileAttributes)
        {
            throw new NotImplementedException("SetFileAttributes not implemented");
        }

        /// <summary>
        /// Initializes the partitions for all block devices.
        /// </summary>
        protected virtual void InitializePartitions()
        {
            Console.WriteLine("Loading partitions...");
            foreach (var t in BlockDevice.Devices)
                if (t is Partition partition)
                    mPartitions.Add(partition);

            if (mPartitions.Count > 0)
            {
                tablePrinter.WriteHeaders("Partition #", "Block Size", "Block Count", "Size");
                for (int i = 0; i < mPartitions.Count; i++)
                {
                    tablePrinter.WriteRow((i + 1).ToString(), mPartitions[i].BlockSize + " bytes", mPartitions[i].BlockCount.ToString(), mPartitions[i].BlockCount * mPartitions[i].BlockSize / 1024 / 1024 + " MB");
                }
            }
            else
            {
                Console.WriteLine("No partitions found!");
            }
        }

        /// <summary>
        /// Initializes the file system for all partitions.
        /// </summary>
        protected virtual void InitializeFileSystems()
        {
            for (int i = 0; i < mPartitions.Count; i++)
            {
                string rootPath = string.Concat(i, VolumeSeparatorChar, DirectorySeparatorChar);
                var size = (long)(mPartitions[i].BlockCount * mPartitions[i].BlockSize / 1024 / 1024);

                Console.WriteLine("Searching file system on partition #" + i);

                foreach (var fs in mRegisteredFileSystems)
                    if (fs.IsType(mPartitions[i]))
                    {
                        mFileSystems.Add(fs.Create(mPartitions[i], rootPath, size));
                        break;
                    }

                if (mFileSystems.Count > 0 && mFileSystems[mFileSystems.Count - 1].RootPath == rootPath)
                {
                    Console.WriteLine(string.Concat("Initialized ", mFileSystems.Count, " filesystem(s)"));
                    Directory.SetCurrentDirectory(rootPath);
                }
                else
                {
                    Console.WriteLine(string.Concat("No filesystem found on partition #", i));
                }
            }
        }

        /// <summary>
        /// Gets the file system from a path.
        /// </summary>
        /// <param name="aPath">The path.</param>
        /// <returns>The file system for the path.</returns>
        /// <exception cref="Exception">Unable to determine filesystem for path:  + aPath</exception>
        private FileSystem GetFileSystemFromPath(string aPath)
        {

            if (String.IsNullOrEmpty(aPath))
            {
                throw new ArgumentException("Argument is null or empty", nameof(aPath));
            }


            string xPath = Path.GetPathRoot(aPath);

            if ((mCurrentFileSystem != null) && (xPath == mCurrentFileSystem.RootPath))
            {
                return mCurrentFileSystem;
            }

            for (int i = 0; i < mFileSystems.Count; i++)
            {
                if (mFileSystems[i].RootPath == xPath)
                {
                    mCurrentFileSystem = mFileSystems[i];
                    return mCurrentFileSystem;
                }
            }
            throw new Exception("Unable to determine filesystem for path: " + aPath);
        }

        /// <summary>
        /// Attempts to get a directory entry for a path in a file system.
        /// </summary>
        /// <param name="aPath">The path.</param>
        /// <param name="aFS">The file system.</param>
        /// <returns>A directory entry for the path.</returns>
        /// <exception cref="ArgumentNullException">aFS</exception>
        /// <exception cref="Exception">Path part ' + xPathPart + ' not found!</exception>
        private DirectoryEntry DoGetDirectoryEntry(string aPath, FileSystem aFS)
        {

            if (String.IsNullOrEmpty(aPath))
            {
                throw new ArgumentException("Argument is null or empty", nameof(aPath));
            }

            if (aFS == null)
            {
                throw new ArgumentNullException(nameof(aFS));
            }


            string[] xPathParts = VFSManager.SplitPath(aPath);

            DirectoryEntry xBaseDirectory = GetVolume(aFS);

            if (xPathParts.Length == 1)
            {
                return xBaseDirectory;
            }

            // start at index 1, because 0 is the volume
            for (int i = 1; i < xPathParts.Length; i++)
            {
                var xPathPart = xPathParts[i].ToLower();

                var xPartFound = false;
                var xListing = aFS.GetDirectoryListing(xBaseDirectory);

                for (int j = 0; j < xListing.Count; j++)
                {
                    var xListingItem = xListing[j];
                    string xListingItemName = xListingItem.mName.ToLower();
                    xPathPart = xPathPart.ToLower();

                    if (xListingItemName == xPathPart)
                    {
                        xBaseDirectory = xListingItem;
                        xPartFound = true;
                        break;
                    }
                }

                if (!xPartFound)
                {
                    throw new Exception("Path part '" + xPathPart + "' not found!");
                }
            }
            return xBaseDirectory;
        }

        /// <summary>
        /// Gets the root directory entry for a volume in a file system.
        /// </summary>
        /// <param name="aFS">The file system containing the volume.</param>
        /// <returns>A directory entry for the volume.</returns>
        private DirectoryEntry GetVolume(FileSystem aFS)
        {

            if (aFS == null)
            {
                throw new ArgumentNullException(nameof(aFS));
            }

            return aFS.GetRootDirectory();
        }

        /// <summary>
        /// Verifies if driveId is a valid id for a drive.
        /// </summary>
        /// <param name="driveId">The id of the drive.</param>
        /// <returns>true if the drive id is valid, false otherwise.</returns>
        public override bool IsValidDriveId(string driveId)
        {

            /* We need to remove ':\' to get only the numeric value */
            driveId = driveId.Remove(driveId.Length - 2);

            /*
             * Cosmos Drive name is really similar to DOS / Windows but a number instead of a letter is used, it is not limited
             * to 1 character but any number is valid
             */

            bool isOK = Int32.TryParse(driveId, out int val);

            return isOK;
        }

        public override long GetTotalSize(string aDriveId)
        {
            var xFs = GetFileSystemFromPath(aDriveId);

            /* We have to return it in bytes */
            return xFs.Size * 1024 * 1024;
        }

        public override long GetAvailableFreeSpace(string aDriveId)
        {
            var xFs = GetFileSystemFromPath(aDriveId);

            return xFs.AvailableFreeSpace;
        }

        public override long GetTotalFreeSpace(string aDriveId)
        {
            var xFs = GetFileSystemFromPath(aDriveId);

            return xFs.TotalFreeSpace;
        }

        public override string GetFileSystemType(string aDriveId)
        {
            var xFs = GetFileSystemFromPath(aDriveId);

            return xFs.Type;
        }

        public override string GetFileSystemLabel(string aDriveId)
        {
            var xFs = GetFileSystemFromPath(aDriveId);

            return xFs.Label;
        }

        public override void SetFileSystemLabel(string aDriveId, string aLabel)
        {

            var xFs = GetFileSystemFromPath(aDriveId);
            xFs.Label = aLabel;
        }

        public override void Format(string aDriveId, string aDriveFormat, bool aQuick)
        {
            var xFs = GetFileSystemFromPath(aDriveId);

            xFs.Format(aDriveFormat, aQuick);
        }

    }
}
