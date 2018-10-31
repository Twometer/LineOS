using System;

namespace LineOS.NTFS.Model.Enums
{
    [Flags]
    public enum FileEntryFlags : ushort
    {
        FileInUse = 1,
        Directory = 2
    }
}
