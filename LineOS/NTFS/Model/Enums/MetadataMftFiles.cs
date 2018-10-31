namespace LineOS.NTFS.Model.Enums
{
    public enum MetadataMftFiles : uint
    {
        Mft = 0,
        MftMirr = 1,
        LogFile = 2,
        Volume = 3,
        AttrDef = 4,
        RootDir = 5,
        Bitmap = 6,
        Boot = 7,
        BadClus = 8,
        Secure = 9,
        Upcase = 10,
        Extend = 11
    }
}