using System.Collections.Generic;

namespace LineOS.NTFS.Utility
{
    public class Util
    {

        public static List<T> Sort<T>(List<T> t, IComparer<T> comparer)
        {
            var array = t.ToArray();
            Quicksort(array, 0, array.Length - 1, comparer);
            for (int i = 0; i < array.Length; i++)
                t[i] = array[i];
            return t;
        }

        private static void Quicksort<T>(T[] data, int left, int right, IComparer<T> comparer)
        {
            int i, j;
            T pivot, temp;
            i = left;
            j = right;
            pivot = data[(left + right) / 2];
            do
            {
                while ((comparer.Compare(data[i], pivot) < 0) && (i < right)) i++;
                while ((comparer.Compare(pivot, data[j]) < 0) && (j > left)) j--;
                if (i <= j)
                {
                    temp = data[i];
                    data[i] = data[j];
                    data[j] = temp;
                    i++;
                    j--;
                }
            } while (i <= j);
            if (left < j) Quicksort(data, left, j, comparer);
            if (i < right) Quicksort(data, i, right, comparer);
        }

    }
}
