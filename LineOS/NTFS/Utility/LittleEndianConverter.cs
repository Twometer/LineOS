﻿using System;

namespace LineOS.NTFS.Utility
{
    public static class LittleEndianConverter
    {
        public static void GetBytes(byte[] buffer, int offset, short value)
        {
            buffer[offset + 0] = (byte)((value >> 0) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        public static void GetBytes(byte[] buffer, int offset, ushort value)
        {
            buffer[offset + 0] = (byte)((value >> 0) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        public static void GetBytes(byte[] buffer, int offset, int value)
        {
            buffer[offset + 0] = (byte)((value >> 0) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        public static void GetBytes(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)((value >> 0) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
        }

        public static void GetBytes(byte[] buffer, int offset, long value)
        {
            buffer[offset + 0] = (byte)((value >> 0) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 4] = (byte)((value >> 32) & 0xFF);
            buffer[offset + 5] = (byte)((value >> 40) & 0xFF);
            buffer[offset + 6] = (byte)((value >> 48) & 0xFF);
            buffer[offset + 7] = (byte)((value >> 56) & 0xFF);
        }

        public static void GetBytes(byte[] buffer, int offset, ulong value)
        {
            buffer[offset + 0] = (byte)((value >> 0) & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
            buffer[offset + 4] = (byte)((value >> 32) & 0xFF);
            buffer[offset + 5] = (byte)((value >> 40) & 0xFF);
            buffer[offset + 6] = (byte)((value >> 48) & 0xFF);
            buffer[offset + 7] = (byte)((value >> 56) & 0xFF);
        }

        public static void GetBytes(byte[] buffer, int offset, DateTime value, DatetimeBinaryFormat format = DatetimeBinaryFormat.WinFileTime)
        {
            switch (format)
            {
                case DatetimeBinaryFormat.WinFileTime:
                    NtfsUtils.ToWinFileTime(buffer, offset, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }

    }

    public enum DatetimeBinaryFormat
    {
        WinFileTime
    }

}
