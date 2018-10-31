using LineOS.FS;
using LineOS.NTFS.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using LineOS.NTFS.IO;
using LineOS.NTFS.Model;
using LineOS.NTFS.Model.Attributes;
using LineOS.NTFS.Model.Enums;

namespace LineOS.NTFS
{
    public class Ntfs
    {
        /*
         * https://github.com/LordMike/NtfsLib/blob/master/NTFSLib/NTFS/NTFSWrapper.cs
         * https://github.com/LordMike/NtfsLib/blob/master/NTFSLib/IO/NtfsFileEntry.cs
         */

        public BootSector BootSector { get; set; }
        public BlockDeviceStream DiskStream { get; set; }

        public uint BytesPerFileRecord { get; private set; }
        public uint FileRecordCount { get; private set; }
        public uint BytesPerCluster => (uint)(BootSector.BytesPerSector * BootSector.SectorsPerCluster);
        public ushort BytesPerSector => BootSector.BytesPerSector;
        public uint SectorsPerRecord { get; private set; }
        public uint SectorsPerCluster => BootSector.SectorsPerCluster;

        public FileRecord MftFile { get; private set; }
        public Stream MftStream { get; private set; }

        private FileRecord[] FileRecords { get; set; }

        public static Ntfs Create(BlockDeviceStream diskStream)
        {
            var ntfs = new Ntfs {DiskStream = diskStream};
            ntfs.Initialize();
            return ntfs;
        }

        private void Initialize()
        {
            byte[] data = new byte[512];
            DiskStream.Seek(0, SeekOrigin.Begin);
            DiskStream.Read(data, 0, data.Length);
            BootSector = BootSector.ParseData(data, 512, 0);

            BytesPerFileRecord = BootSector.MftRecordSizeBytes;
            SectorsPerRecord = BytesPerFileRecord / BytesPerSector;

            MftFile = ParseMftRecordData(ReadMftRecordData((uint)MetadataMftFiles.Mft));
            MftStream = OpenFileRecord(MftFile);

            AttributeData mftFileData = null;
            foreach (var att in MftFile.Attributes)
                if (att is AttributeData attributeData)
                    mftFileData = attributeData;

            if (mftFileData == null)
                throw new Exception("ntfs error: unable to load mft data");

            long clusterBytes = 0;
            foreach (var frag in mftFileData.DataFragments)
                clusterBytes += frag.Clusters * BytesPerCluster;

            FileRecordCount = (uint)(clusterBytes / BytesPerFileRecord);
            FileRecords = new FileRecord[FileRecordCount];
            FileRecords[0] = MftFile;

            Console.WriteLine("[NTFSDRV2] Initialized with " + FileRecordCount + " file records");
        }

        public FileRecord ReadMftRecord(uint number, bool parseAttributeLists = true)
        {
            if (number <= FileRecords.Length && FileRecords[number] != null)
                return FileRecords[number];

            var data = ReadMftRecordData(number);
            var record = ParseMftRecordData(data);

            FileRecords[number] = record;


            if (BytesPerFileRecord == 0)
                BytesPerFileRecord = record.SizeOfFileRecordAllocated;

            if (parseAttributeLists)
                LoadAttributeLists(record);

            return record;
        }

        private void LoadAttributeLists(FileRecord record)
        {
            if (record.ExternalAttributes.Count > 0)
                return;

            var completedFiles = new List<FileReference>();
            foreach (var att in record.Attributes)
            {
                if (!(att is AttributeList listAttr)) continue;

                if(listAttr.NonResidentFlag == ResidentFlag.NonResident)
                    listAttr.ParseAttributeNonResidentBody(this);

                foreach (var listItem in listAttr.Items)
                {
                    if(listItem.BaseFile.Equals(record.FileReference) || completedFiles.Contains(listItem.BaseFile))
                        continue;
                    completedFiles.Add(listItem.BaseFile);

                    var otherRecord = ReadMftRecord(listItem.BaseFile.FileId);
                    record.ParseExternalAttributes(otherRecord);
                }
            }
        }

        public byte[] ReadMftRecordData(uint number)
        {
            var length = BytesPerFileRecord == 0 ? 4096 : BytesPerFileRecord;
            long offset = number * length;
            if (MftFile == null)
                offset += (long)(BootSector.MftCluster * BytesPerCluster);
            else if (MftStream != null)
            {
                var mftData = new byte[length];
                MftStream.Seek(offset, SeekOrigin.Begin);
                MftStream.Read(mftData, 0, mftData.Length);
                return mftData;
            }

            var data = new byte[length];
            DiskStream.Seek(offset, SeekOrigin.Begin);
            DiskStream.Read(data, 0, data.Length);
            return data;
        }

        public FileRecord ParseMftRecordData(byte[] data)
        {
            return FileRecord.Parse(data, 0, BytesPerSector, SectorsPerRecord);
        }

        private Stream OpenFileRecord(FileRecord record, string dataStream = "")
        {
            LoadAttributeLists(record);

            var dataAttribs = new List<AttributeData>();
            foreach (var attrib in record.Attributes)
                if (attrib is AttributeData data && attrib.AttributeName == dataStream)
                    dataAttribs.Add(data);

            if (dataAttribs.Count == 1 && dataAttribs[0].NonResidentFlag == ResidentFlag.Resident)
                return new MemoryStream(dataAttribs[0].DataBytes);

            var fragments = new List<DataFragment>();
            foreach (var attrib in dataAttribs)
                foreach (var frag in attrib.DataFragments)
                    fragments.Add(frag);

            var compressionUnitSize = dataAttribs[0].NonResidentHeader.CompressionUnitSize;
            var compressionClusterCount = (ushort)(compressionUnitSize == 0 ? 0 : Math.Pow(2, compressionUnitSize));
            var contentSize = (long)dataAttribs[0].NonResidentHeader.ContentSize;

            return new NtfsDiskStream(DiskStream, false, fragments, BytesPerCluster, compressionClusterCount, contentSize);
        }

        public NtfsDirectory GetRootDirectory()
        {
            return (NtfsDirectory) NtfsFileEntry.CreateEntry(this, (uint) MetadataMftFiles.RootDir);
        }

        public void ParseNonResidentAttribute(AttributeIndexAllocation alloc)
        {
            if(alloc.NonResidentHeader.Fragments.Count > 0)   
                alloc.ParseAttributeNonResidentBody(this);
        }
    }
}
