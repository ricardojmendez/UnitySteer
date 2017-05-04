using System;


public class LSAssert
{
	public static bool AreEqual<T> (T a, T b){
		if (a is IComparable && b is IComparable) {
			return ((IComparable)a).CompareTo((IComparable)b) == 0;
		}

		return a.Equals(b);
	}
}

