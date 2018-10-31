using System;
using System.Collections.Generic;
using System.IO;
using LineOS.NTFS.IO;
using LineOS.NTFS.Model;
using LineOS.NTFS.Model.Attributes;
using LineOS.NTFS.Model.Enums;

namespace LineOS.NTFS.Utility
{
    public static class NtfsUtils
    {

        private static readonly long MaxFileTime = 9999; // DateTime.MaxValue.ToFileTimeUtc();

        public static DateTime FromWinFileTime(byte[] data, int offset)
        {
            /*long fileTime = BitConverter.ToInt64(data, offset);

            if (fileTime >= MaxFileTime)
                return DateTime.MaxValue;*/

            return DateTime.MaxValue;
        }

        public static void ToWinFileTime(byte[] data, int offset, DateTime dateTime)
        {
            if (dateTime == DateTime.MaxValue)
            {
                LittleEndianConverter.GetBytes(data, offset, long.MaxValue);
            }
            else
            {
                long fileTime = 0; // dateTime.ToFileTimeUtc();

                LittleEndianConverter.GetBytes(data, offset, fileTime);
            }
        }

        public static byte[] ReadFragments(Ntfs ntfsInfo, List<DataFragment> fragments)
        {
            long totalLength = 0; 
            foreach (var frag in fragments)
                totalLength += frag.Clusters * ntfsInfo.BytesPerCluster;

            var data = new byte[totalLength];

            Stream diskStream = ntfsInfo.DiskStream;
            using (NtfsDiskStream stream = new NtfsDiskStream(diskStream, false, fragments, ntfsInfo.BytesPerCluster, 0, totalLength))
            {
                stream.Read(data, 0, data.Length);
            }

            // Return the data
            return data;
        }

        public static void ApplyUSNPatch(byte[] data, int offset, uint sectors, ushort bytesPrSector, byte[] usnNumber, byte[] usnData)
        {
            // // Debug.Assert(data.Length >= offset + sectors * bytesPrSector);
            // // Debug.Assert(usnNumber.Length == 2);
            // // Debug.Assert(sectors * 2 <= usnData.Length);

            for (int i = 0; i < sectors; i++)
            {
                // Get pointer to the last two bytes
                int blockOffset = offset + i * bytesPrSector + 510;

                // Check that they match the USN Number
                // // Debug.Assert(data[blockOffset] == usnNumber[0]);
                // // Debug.Assert(data[blockOffset + 1] == usnNumber[1]);

                // Patch in new data
                data[blockOffset] = usnData[i * 2];
                data[blockOffset + 1] = usnData[i * 2 + 1];
            }
        }
        public static AttributeFileName GetPreferredDisplayName(FileRecord record)
        {
            AttributeFileName posix = null;
            AttributeFileName win32 = null;
            AttributeFileName dos = null;
            AttributeFileName win32AndDos = null;
            foreach (var a in record.Attributes)
                if (a is AttributeFileName fileName)
                    switch (fileName.FilenameNamespace)
                    {
                        case FileNamespace.POSIX:
                            posix = fileName;
                            break;
                        case FileNamespace.DOS:
                            dos = fileName;
                            break;
                        case FileNamespace.Win32:
                            win32 = fileName;
                            break;
                        case FileNamespace.Win32AndDOS:
                            win32AndDos = fileName;
                            break;
                    }

            if (win32 != null)
                return win32;
            if (win32AndDos != null)
                return win32AndDos;
            if (posix != null)
                return posix;
            if (dos != null)
                return dos;

            throw new Exception("ntfs: invalid filename attribute");
        }

        /*public static AttributeFileName GetPreferredDisplayName(FileRecord record)
        {
            return GetPreferredDisplayName(record.Attributes);
        }

        public static AttributeFileName GetPreferredDisplayName(IEnumerable<Attribute> attributes)
        {
            return attributes.OfType<AttributeFileName>().OrderByDescending(s => s.FilenameNamespace, new FileNamespaceComparer()).FirstOrDefault();
        }*/
    }
}
