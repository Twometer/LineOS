using System;
using System.Collections.Generic;
using System.IO;
using LineOS.NTFS.IO;
using LineOS.NTFS.Model;
using LineOS.NTFS.Model.Attributes;
using LineOS.NTFS.Model.Enums;

namespace LineOS.NTFS.Parser
{
    public class NtfsParser
    {
        public BootSector BootSector { get; private set; }

        public uint SectorsPerRecord { get; private set; }

        public uint BytesPerFileRecord { get;  set; }
        public uint BytesPerCluster => (uint)(BootSector.BytesPerSector * BootSector.SectorsPerCluster);
        public uint BytesPerSector => BootSector.BytesPerSector;
        public byte SectorsPerCluster => BootSector.SectorsPerCluster;
        public ulong TotalSectors => BootSector.TotalSectors;
        public ulong TotalClusters => BootSector.TotalSectors / BootSector.SectorsPerCluster;

        public uint CurrentMftRecordNumber { get; set; }
        public uint FileRecordCount { get; private set; }

        public bool OwnsDiskStream => false;
        public Stream DiskStream { get; }

        private FileRecord mftRecord;
        private NtfsDiskStream mftStream;

        private byte[] buffer;


        public NtfsParser(Stream diskStream)
        {
            this.DiskStream = diskStream;
            ParseBootSector();
            ParseMft();
        }

        private void ParseBootSector()
        {
            var data = new byte[512];
            DiskStream.Seek(0, SeekOrigin.Begin);
            DiskStream.Read(data, 0, data.Length);

            BootSector = BootSector.ParseData(data, data.Length, 0);
            BytesPerFileRecord = BootSector.MftRecordSizeBytes;
            SectorsPerRecord = BootSector.MftRecordSizeBytes / BootSector.BytesPerSector;
        }

        private void ParseMft()
        {
            buffer = new byte[BytesPerFileRecord];
            DiskStream.Seek((long)(BootSector.MftCluster * BytesPerCluster), SeekOrigin.Begin);
            DiskStream.Read(buffer, 0, buffer.Length);
            mftRecord = FileRecord.Parse(buffer, 0, BootSector.BytesPerSector, SectorsPerRecord);

            var fragmentList = new List<DataFragment>();
            AttributeData firstAttributeData = null;
             foreach (var attribute in mftRecord.Attributes)
                 if (attribute is AttributeData data && data.AttributeName == string.Empty &&
                     data.NonResidentFlag == ResidentFlag.NonResident)
                 {
                     if (firstAttributeData == null)
                         firstAttributeData = data;
                     foreach (var frag in data.DataFragments)
                         fragmentList.Add(frag);
                 }
            
           fragmentList = Utility.Util.Sort(fragmentList, new DataFragmentComparer());
           ushort compressionUnitSize = firstAttributeData.NonResidentHeader.CompressionUnitSize;
           ushort compressionClusterCount = (ushort)(compressionUnitSize == 0 ? 0 : Math.Pow(2, compressionUnitSize));
           mftStream = new NtfsDiskStream(DiskStream, false, fragmentList, BytesPerCluster, compressionClusterCount, (long)firstAttributeData.NonResidentHeader.ContentSize);

           CurrentMftRecordNumber = 0;
           FileRecordCount = (uint)(mftStream.Length / BytesPerFileRecord);
            Console.WriteLine("[NTFS DRIVER] " + FileRecordCount + " file records found");
        }

        public FileRecord NextMFTRecord()
        {
            var newPosition = CurrentMftRecordNumber * BytesPerFileRecord;
            if (mftStream.Position != newPosition)
                mftStream.Seek(newPosition, SeekOrigin.Begin);
            var read = mftStream.Read(buffer, 0, buffer.Length);

            if (read == 0)
                return null;

            var record = FileRecord.Parse(buffer, 0, BootSector.BytesPerSector, SectorsPerRecord);
            CurrentMftRecordNumber = record.MFTNumber + 1;
            return record;
        }
    }
}
