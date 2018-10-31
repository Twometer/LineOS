using Cosmos.HAL.BlockDevice;
using Cosmos.System.FileSystem;

namespace LineOS.NTFS.Cosmos
{
    public class NtfsFileSystemFactory : FileSystemFactory
    {
        public override bool IsType(Partition aDevice)
        {
            byte[] block = aDevice.NewBlockArray(1);
            aDevice.ReadBlock(0, 1, block);
            if (block[3] == 0x4E && block[4] == 0x54 && block[5] == 0x46 && block[6] == 0x53)
                return true;
            return false;
        }

        public override FileSystem Create(Partition aDevice, string aRootPath, long aSize)
        {
            return new NtfsFileSystem(aDevice, aRootPath, aSize);
        }

        public override string Name => "NTFS";
    }
}
