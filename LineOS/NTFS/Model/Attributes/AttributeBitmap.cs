﻿using System;
using System.Collections;
using LineOS.NTFS.Model.Enums;
using LineOS.NTFS.Utility;

namespace LineOS.NTFS.Model.Attributes
{
    public class AttributeBitmap : Attribute
    {
        public BitArray Bitfield { get; set; }

        public override AttributeResidentAllow AllowedResidentStates
        {
            get
            {
                return AttributeResidentAllow.Resident | AttributeResidentAllow.NonResident;
            }
        }

        internal override void ParseAttributeResidentBody(byte[] data, int maxLength, int offset)
        {
            base.ParseAttributeResidentBody(data, maxLength, offset);

            // Debug.Assert(maxLength >= 1);

            byte[] tmpData = new byte[maxLength];
            Array.Copy(data, offset, tmpData, 0, maxLength);

            Bitfield = new BitArray(tmpData);
        }

        internal override void ParseAttributeNonResidentBody(Ntfs ntfsInfo)
        {
            base.ParseAttributeNonResidentBody(ntfsInfo);

            // Get all chunks
            byte[] data = NtfsUtils.ReadFragments(ntfsInfo, NonResidentHeader.Fragments);
            
            // Parse
            Bitfield = new BitArray(data);
        }
    }
}