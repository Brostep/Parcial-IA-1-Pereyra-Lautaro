using System;
using System.Collections.Generic;

namespace IA2.FP {
	public static class Generators {
		public static IEnumerable<Src> Generate<Src>(Src seed, Func<Src, Src> generator) {
			while (true) {
				if (seed == null)
					yield break;
				
				yield return seed;
				seed = generator(seed);
			}
		}
	}
}
