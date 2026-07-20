using System;
using System.Collections.Generic;

namespace RCP
{
    public interface IRange
    {
        object Lower { get; }
        object Upper { get; }
    }

    public struct Range<T> : IEquatable<Range<T>>, IRange
    {
        public T Lower, Upper;

        public Range(T lower, T upper)
        {
            Lower = lower;
            Upper = upper;
        }

        object IRange.Lower => Lower;
        object IRange.Upper => Upper;

        public override string ToString() => $"{Lower} : {Upper}";

        public override bool Equals(object obj)
        {
            if (obj is Range<T>)
                return Equals((Range<T>)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode() => Lower.GetHashCode() ^ Upper.GetHashCode();

        public bool Equals(Range<T> other)
        {
            var comparer = EqualityComparer<T>.Default;
            return comparer.Equals(Lower, other.Lower) && comparer.Equals(Upper, other.Upper);
        }

        public static bool operator ==(Range<T> a, Range<T> b) => a.Equals(b);
        public static bool operator !=(Range<T> a, Range<T> b) => !a.Equals(b);
    }
}
