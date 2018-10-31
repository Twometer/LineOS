using LineOS.NTFS.Model;
using LineOS.NTFS.Model.Attributes;

namespace LineOS.NTFS.IO
{
    public class NtfsFile : NtfsFileEntry
    {
        internal NtfsFile(Ntfs ntfs, FileRecord record, AttributeFileName fileName)
            : base(ntfs, record, fileName)
        {
        }

        public override string ToString()
        {
            return FileName.FileName;
        }
    }
}