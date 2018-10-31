using System.Collections.Generic;
using LineOS.NTFS.Model;

namespace LineOS.NTFS.IO
{
    public class DataFragmentComparer : IComparer<DataFragment>
    {
        public int Compare(DataFragment x, DataFragment y)
        {
            if (x != null && y != null) return x.StartingVCN.CompareTo(y.StartingVCN);
            return 0;
        }
    }
}
