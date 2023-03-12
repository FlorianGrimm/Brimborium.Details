namespace System.Collections.Generic;

public static class ListExtensions {
    public static int BinarySearch<TItem, TMatch>(this List<TItem> list, TMatch match, Func<TItem, TMatch, int> comparer) {
        int lo = 0;
        int hi = list.Count - 1;
        while (lo <= hi) {
            int i = lo + ((hi - lo) >> 1);
            int order = comparer(list[i], match);

            if (order == 0) {
                return i;
            } else if (order < 0) {
                lo = i + 1;
            } else {
                hi = i - 1;
            }
        }
        return ~lo;
    }
}