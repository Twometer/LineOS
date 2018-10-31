using System;
using LineOS.NTFS.Model;
using LineOS.NTFS.Model.Attributes;
using LineOS.NTFS.Model.Enums;
using LineOS.NTFS.Utility;

namespace LineOS.NTFS.IO
{
    public abstract class NtfsFileEntry
    {
        protected Ntfs ntfs;
        public FileRecord MFTRecord { get; private set; }

        internal AttributeFileName FileName;
        private AttributeStandardInformation _standardInformation;

        public DateTime TimeCreation => _standardInformation == null ? DateTime.MinValue : _standardInformation.TimeCreated;

        public DateTime TimeModified => _standardInformation == null ? DateTime.MinValue : _standardInformation.TimeModified;

        public DateTime TimeAccessed => _standardInformation == null ? DateTime.MinValue : _standardInformation.TimeAccessed;

        public DateTime TimeMftModified => _standardInformation == null ? DateTime.MinValue : _standardInformation.TimeMftModified;

        public string Name => FileName.FileName;

        public NtfsDirectory Parent => CreateEntry(FileName.ParentDirectory.FileId) as NtfsDirectory;

        protected NtfsFileEntry(Ntfs ntfs, FileRecord record, AttributeFileName fileName)
        {
            this.ntfs = ntfs;
            MFTRecord = record;
            FileName = fileName;
            Init();
        }

        private void Init()
        {
            foreach(var att in MFTRecord.Attributes)
                if (att is AttributeStandardInformation info)
                    _standardInformation = info;
        }

        internal NtfsFileEntry CreateEntry(uint fileId, AttributeFileName fileName = null)
        {
            return CreateEntry(ntfs, fileId, fileName);
        }

        internal static NtfsFileEntry CreateEntry(Ntfs ntfs, uint fileId, AttributeFileName fileName = null)
        {
            var record = ntfs.ReadMftRecord(fileId);
            if (fileName == null)
                fileName = NtfsUtils.GetPreferredDisplayName(record);

            if ((record.Flags & FileEntryFlags.Directory) != 0)
                return new NtfsDirectory(ntfs, record, fileName);
            return new NtfsFile(ntfs, record, fileName);
        }

    }
}