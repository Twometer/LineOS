using System.Collections.Generic;
using LineOS.NTFS.Model.Enums;
using LineOS.NTFS.Utility;

namespace LineOS.NTFS.Model.Attributes
{
    public class AttributeIndexAllocation : Attribute
    {
        public List<IndexAllocationChunk> Indexes { get; set; }
        public List<IndexEntry> Entries { get; set; }

        public override AttributeResidentAllow AllowedResidentStates
        {
            get
            {
                return AttributeResidentAllow.NonResident;
            }
        }

        internal override void ParseAttributeNonResidentBody(Ntfs ntfsInfo)
        {
            byte[] data = NtfsUtils.ReadFragments(ntfsInfo, NonResidentHeader.Fragments);

            List<IndexAllocationChunk> indexes = new List<IndexAllocationChunk>();
            List<IndexEntry> entries = new List<IndexEntry>();

            // Parse
            for (int i = 0; i < NonResidentHeader.Fragments.Count; i++)
            {
                for (int j = 0; j < NonResidentHeader.Fragments[i].Clusters; j++)
                {
                    int offset = (int)((NonResidentHeader.Fragments[i].StartingVCN - NonResidentHeader.Fragments[0].StartingVCN) * ntfsInfo.BytesPerCluster + j * ntfsInfo.BytesPerCluster);

                    if (!IndexAllocationChunk.IsIndexAllocationChunk(data, offset))
                        continue;

                    IndexAllocationChunk index = IndexAllocationChunk.ParseBody(ntfsInfo, data, offset);

                    indexes.Add(index);
                    foreach(var f in index.Entries)
                        entries.Add(f);
                }
            }

            Indexes = indexes;
            Entries = entries;
        }
    }
}