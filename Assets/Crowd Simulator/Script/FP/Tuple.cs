using System;
using System.Linq;
using System.Collections.Generic;

namespace IA2.FP {
	public struct Tuple<T1, T2> {
		public T1 First { get; private set; }
		public T2 Second { get; private set; }
		
		internal Tuple(T1 first, T2 second) {
			First = first;
			Second = second;
		}

		public override string ToString() {
			return "(" + First.ToString() + ", " + Second.ToString() + ")";
		}
	}

	public struct Tuple<T1, T2, T3> {
		public T1 First { get; private set; }
		public T2 Second { get; private set; }
		public T3 Third { get; private set; }

		internal Tuple(T1 first, T2 second, T3 third) {
			First = first;
			Second = second;
			Third = third;
		}
		
		public override string ToString() {
			return "(" + First.ToString() + ", " + Second.ToString() + ", " + Third.ToString() + ")";
		}
	}

	public struct Tuple<T1, T2, T3, T4> {
		public T1 First { get; private set; }
		public T2 Second { get; private set; }
		public T3 Third { get; private set; }
		public T4 Fourth { get; private set; }


		internal Tuple(T1 first, T2 second, T3 third, T4 fourth) {
			First = first;
			Second = second;
			Third = third;
			Fourth = fourth;
		}
		
		public override string ToString() {
			return "(" + First.ToString() + ", " + Second.ToString() + ", " + Third.ToString() + ", " + Fourth.ToString() + ")";
		}
	}

	public static class Tuple {
		public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second) {
			return new Tuple<T1, T2>(first, second);
		}

		public static Tuple<T1, T2, T3> New<T1, T2, T3>(T1 first, T2 second, T3 third) {
			return new Tuple<T1, T2, T3>(first, second, third);
		}

		public static Tuple<T1, T2, T3, T4> New<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth) {
			return new Tuple<T1, T2, T3, T4>(first, second, third, fourth);
		}
	}

	public static class Extension { 
		public static IEnumerable<T> Enumerate<T>(this Tuple<T, T> tuple) {
			yield return tuple.First;
			yield return tuple.Second;
		}

		public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T> tuple) {
			yield return tuple.First;
			yield return tuple.Second;
			yield return tuple.Third;
		}

		public static IEnumerable<T> Enumerate<T>(this Tuple<T, T, T, T> tuple) {
			yield return tuple.First;
			yield return tuple.Second;
			yield return tuple.Third;
			yield return tuple.Fourth;
		}

		public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(
			this IEnumerable<T1> list1,
			IEnumerable<T2> list2) {

			var e2 = list2.GetEnumerator();

			foreach (var e1 in list1) {
				e2.MoveNext ();
				yield return Tuple.New(e1, e2.Current);
			}
		}

		public static IEnumerable<Tuple<T1, T2, T3>> Zip<T1, T2, T3>(
			this IEnumerable<T1> list1,
			IEnumerable<T2> list2,
			IEnumerable<T3> list3) {

			var e2 = list2.GetEnumerator();
			var e3 = list3.GetEnumerator();

			foreach (var e1 in list1) {
				e2.MoveNext();
				e3.MoveNext();
				yield return Tuple.New(e1, e2.Current, e3.Current);
			}
		}
	}
}
