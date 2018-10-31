using System;
using System.Text;

namespace LineOS.NTFS.Parser
{
    /*
     * The MIT License (MIT)
     * 
     * Copyright (c) 2015 Michael Bisbjerg
     * 
     * Permission is hereby granted, free of charge, to any person obtaining a copy
     * of this software and associated documentation files (the "Software"), to deal
     * in the Software without restriction, including without limitation the rights
     * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
     * copies of the Software, and to permit persons to whom the Software is
     * furnished to do so, subject to the following conditions:
     * 
     * The above copyright notice and this permission notice shall be included in all
     * copies or substantial portions of the Software.
     * 
     * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
     * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
     * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
     * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
     * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
     * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
     * SOFTWARE.
     */
    public class BootSector
    {
        public byte[] JmpInstruction { get; set; }
        public string OemCode { get; set; }
        public ushort BytesPerSector { get; set; }
        public byte SectorsPerCluster { get; set; }
        public ushort ReservedSectors { get; set; }
        public byte MediaDescriptor { get; set; }
        public ushort SectorsPerTrack { get; set; }
        public ushort NumberOfHeads { get; set; }
        public uint HiddenSectors { get; set; }
        public uint Usually80008000 { get; set; }
        public ulong TotalSectors { get; set; }
        public ulong MftCluster { get; set; }
        public ulong MftMirrCluster { get; set; }
        public uint MftRecordSizeBytes { get; set; }
        public uint MftIndexSizeBytes { get; set; }
        public ulong SerialNumber { get; set; }
        public uint Checksum { get; set; }
        public byte[] BootstrapCode { get; set; }
        public byte[] Signature { get; set; }

        public static BootSector ParseData(byte[] data, int maxLength, int offset)
        {
            //// Debug.Assert(data.Length - offset >= 512);
            //// Debug.Assert(0 <= offset && offset <= data.Length);

            BootSector res = new BootSector();

            res.JmpInstruction = new byte[3];
            Array.Copy(data, offset, res.JmpInstruction, 0, 3);

            res.OemCode = Encoding.ASCII.GetString(data, offset + 3, 8).Trim();
            res.BytesPerSector = BitConverter.ToUInt16(data, offset + 11);
            res.SectorsPerCluster = data[offset + 13];
            res.ReservedSectors = BitConverter.ToUInt16(data, offset + 14);
            res.MediaDescriptor = data[offset + 21];
            res.SectorsPerTrack = BitConverter.ToUInt16(data, offset + 24);
            res.NumberOfHeads = BitConverter.ToUInt16(data, offset + 26);
            res.HiddenSectors = BitConverter.ToUInt32(data, offset + 28);
            res.Usually80008000 = BitConverter.ToUInt32(data, offset + 36);
            res.TotalSectors = BitConverter.ToUInt64(data, offset + 40);
            res.MftCluster = BitConverter.ToUInt64(data, offset + 48);
            res.MftMirrCluster = BitConverter.ToUInt64(data, offset + 56);
            res.MftRecordSizeBytes = BitConverter.ToUInt32(data, offset + 64);
            res.MftIndexSizeBytes = BitConverter.ToUInt32(data, offset + 68);
            res.SerialNumber = BitConverter.ToUInt64(data, offset + 72);
            res.Checksum = BitConverter.ToUInt32(data, offset + 80);

            res.MftRecordSizeBytes = InterpretClusterCount(res.MftRecordSizeBytes);
            res.MftIndexSizeBytes = InterpretClusterCount(res.MftRecordSizeBytes);

            res.BootstrapCode = new byte[426];
            Array.Copy(data, offset + 84, res.BootstrapCode, 0, 426);

            res.Signature = new byte[2];
            Array.Copy(data, offset + 510, res.Signature, 0, 2);

            // Signature should always be this
            //// Debug.Assert(res.Signature[0] == 0x55);
            //// Debug.Assert(res.Signature[1] == 0xAA);

            return res;
        }

        private static uint InterpretClusterCount(uint num)
        {
            // Find if this number is negative, taking into account the number of bytes needed to store it
            int bytes = 0;
            for (int i = 0; i < 4; i++)
            {
                if (num >= ((uint)0xFF << (i * 8)))
                {
                    bytes = i + 1;
                }
            }

            // Is it negative?
            uint negativeNum = 0x80;
            for (int i = 0; i < bytes; i++)
            {
                negativeNum = negativeNum << 8;
            }

            if ((negativeNum & num) != negativeNum)
                // Not negative, return as-is
                return num;

            int newNumber = (int)num;
            for (int i = bytes + 1; i < 4; i++)
            {
                newNumber |= 0xFF << (i * 8);
            }

            // Calculate count
            // 2^(-1 * -10) 
            uint res = (uint)Math.Pow(2, -newNumber);

            return res;
        }
    }
}
