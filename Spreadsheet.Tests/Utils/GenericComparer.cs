using System;
using System.Collections;
using System.Collections.Generic;

namespace Spreadsheet.Tests
{
    public class GenericComparer<T> : IComparer<T>, IComparer
    {
        private readonly Func<T, T, int> _comparer;

        public GenericComparer(Func<T,T,int> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }

        public int Compare(object x, object y)
        {
            return _comparer((T) x, (T) y);
        }
    }
}