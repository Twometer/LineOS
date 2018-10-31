using System;

namespace LineOS.NTFS.Model.Enums
{
    [Flags]
    public enum AttributeResidentAllow
    {
        Resident = 1,
        NonResident = 2
    }
}