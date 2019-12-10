using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IA2.FP {
	public class FList<T> : IEnumerable<T> {
		IEnumerable<T> collection;
		
		public FList () {
			collection = new T[0];
		}
		
		public FList(IEnumerable<T> collection) {
			this.collection = collection;
		}
		
		public static FList<T> operator+(FList<T> list, IEnumerable<T> other) {
			return FList.New(list.collection.Concat(other));
		}
		
		public static FList<T> operator+(FList<T> list, T element) {
			return FList.New(list.collection.Concat(new T[] { element }));
		}
		
		public IEnumerator<T> GetEnumerator() {
			return GetEnumeratorImplementation();
		}
		
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumeratorImplementation();
		}
		
		IEnumerator<T> GetEnumeratorImplementation() {
			foreach(var element in collection)
				yield return element;
		}
	}

	public static class FList {
		static public FList<T> New<T>() {
			return new FList<T>();
		}
		
		static public FList<T> New<T>(IEnumerable<T> collection) {
			return new FList<T>(collection);
		}
	}
}
