using System.Collections.Generic;

namespace IA2.FP {
	public static class FPExtensions {
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) {
			return new HashSet<T>(source);
		}
	}
}