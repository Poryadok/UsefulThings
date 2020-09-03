using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Extensions
{
	public static class CollectionExtensions
	{
		public static bool IsEmpty<T>(this T collection) where T : ICollection
		{
			return (collection == null) || (collection.Count == 0);
		}

		public static bool IsEmpty<T>(this ICollection<T> collection)
		{
			return (collection == null) || (collection.Count == 0);
		}

		public static bool AddNoDoubling<T>(this ICollection<T> collection, T item)
		{
			if (collection == null)
			{
				return false;
			}

			if (collection.Contains(item))
			{
				return false;
			}

			collection.Add(item);
			return true;
		}

		public static void AddRangeNoDoubling<T>(this ICollection<T> collection, ICollection<T> range)
		{
			if (collection == null || range == null || range.Count == 0)
			{
				return;
			}

			foreach (var item in range)
			{
				if (collection.Contains(item))
				{
					continue;
				}

				collection.Add(item);
			}
		}

		public static int CountSelect<T>(this ICollection<T> collection, Predicate<T> predicate)
		{
			if (collection.IsEmpty())
			{
				return 0;
			}

			var value = 0;

			foreach (var item in collection)
			{
				if (predicate(item))
				{
					value++;
				}
			}

			return value;
		}

		public static T FindWithCast<T>(this ICollection collection, Predicate<T> predicate) where T : class
		{
			if (collection.IsEmpty())
			{
				return default(T);
			}

			foreach (var item in collection)
			{
				if (item is T u && predicate(u))
				{
					return u;
				}
			}
			return default(T);
		}

		public static T FindSmallest<T>(this ICollection<T> collection, Predicate<T> predicate, Func<T, int> evaluator)
		{
			if (collection.IsEmpty())
			{
				return default(T);
			}

			bool hasResult = false;
			var value = int.MaxValue;
			T result = default(T);

			foreach (var item in collection)
			{
				if (predicate(item))
				{
					if (hasResult)
					{
						if (evaluator(item) < value)
						{
							result = item;
							value = evaluator(item);
						}
					}
					else
					{
						value = evaluator(item);
						result = item;
						hasResult = true;
					}
				}
			}
			return result;
		}

		public static KeyValuePair<T, U> Find<T, U>(this IDictionary<T, U> collection, Predicate<KeyValuePair<T, U>> predicate)
		{
			if (collection.IsEmpty())
			{
				return default(KeyValuePair<T, U>);
			}

			foreach (var item in collection)
			{
				if (predicate(item))
				{
					return item;
				}
			}
			return default(KeyValuePair<T, U>);
		}

		public static T FindSmallest<T>(this ICollection<T> collection, Predicate<T> predicate, Func<T, float> evaluator)
		{
			if (collection.IsEmpty())
			{
				return default(T);
			}

			bool hasResult = false;
			var value = float.MaxValue;
			T result = default(T);

			foreach (var item in collection)
			{
				if (predicate(item))
				{
					if (hasResult)
					{
						if (evaluator(item) < value)
						{
							result = item;
							value = evaluator(item);
						}
					}
					else
					{
						value = evaluator(item);
						result = item;
						hasResult = true;
					}
				}
			}
			return result;
		}

		public static T FindBiggest<T>(this ICollection<T> collection, Predicate<T> predicate, Func<T, int> evaluator)
		{
			if (collection.IsEmpty())
			{
				return default(T);
			}

			bool hasResult = false;
			var value = int.MinValue;
			T result = default(T);

			foreach (var item in collection)
			{
				if (predicate(item))
				{
					if (hasResult)
					{
						if (evaluator(item) > value)
						{
							value = evaluator(item);
							result = item;
						}
					}
					else
					{
						value = evaluator(item);
						result = item;
						hasResult = true;
					}
				}
			}
			return result;
		}

		public static T FindBiggest<T>(this ICollection<T> collection, Predicate<T> predicate, Func<T, float> evaluator)
		{
			if (collection.IsEmpty())
			{
				return default(T);
			}

			bool hasResult = false;
			var value = float.MinValue;
			T result = default(T);

			foreach (var item in collection)
			{
				if (predicate(item))
				{
					if (hasResult)
					{
						if (evaluator(item) > value)
						{
							value = evaluator(item);
							result = item;
						}
					}
					else
					{
						value = evaluator(item);
						result = item;
						hasResult = true;
					}
				}
			}
			return result;
		}

		public static bool TryFind<T, V>(this Dictionary<T, V> collection, Predicate<V> predicate, out KeyValuePair<T, V> result) where V : class
		{
			if (collection.IsEmpty())
			{
				result = default(KeyValuePair<T, V>);
				return false;
			}

			foreach (var pair in collection)
			{
				if (predicate(pair.Value))
				{
					result = pair;
					return true;
				}
			}

			result = default(KeyValuePair<T, V>);
			return false;
		}

		public static bool Any<T, V>(this Dictionary<T, V> collection, Predicate<V> predicate)
		{
			if (collection.IsEmpty())
				return false;

			foreach (var pair in collection)
			{
				if (predicate(pair.Value))
				{
					return true;
				}
			}

			return false;
		}

		private static void Swap<T>(this IList<T> collection, int a, int b)
		{
			var tmp = collection[a];
			collection[a] = collection[b];
			collection[b] = tmp;
		}

		public static void Sort<T>(this IList<T> collection, Func<T, int> compareFunc, int firstIndex = 0, int lastIndex = int.MinValue)
		{
			if (lastIndex == int.MinValue)
			{
				lastIndex = collection.Count - 1;
			}
			if (firstIndex >= lastIndex)
			{
				return;
			}

			int middleIndex = (lastIndex - firstIndex) / 2 + firstIndex, currentIndex = firstIndex;

			collection.Swap(firstIndex, middleIndex);

			for (int i = firstIndex + 1; i <= lastIndex; ++i)
			{
				if (compareFunc((T)collection[i]) < compareFunc((T)collection[firstIndex]))
				{
					collection.Swap(++currentIndex, i);
				}
			}

			collection.Swap(firstIndex, currentIndex);

			Sort(collection, compareFunc, firstIndex, currentIndex - 1);
			Sort(collection, compareFunc, currentIndex + 1, lastIndex);
		}

		public static void Sort<T>(this IList<T> collection, Func<T, float> compareFunc, int firstIndex = 0, int lastIndex = int.MinValue)
		{
			if (lastIndex == int.MinValue)
			{
				lastIndex = collection.Count - 1;
			}
			if (firstIndex >= lastIndex)
			{
				return;
			}

			int middleIndex = (lastIndex - firstIndex) / 2 + firstIndex, currentIndex = firstIndex;

			collection.Swap(firstIndex, middleIndex);

			for (int i = firstIndex + 1; i <= lastIndex; ++i)
			{
				if (compareFunc((T)collection[i]) < compareFunc((T)collection[firstIndex]))
				{
					collection.Swap(++currentIndex, i);
				}
			}

			collection.Swap(firstIndex, currentIndex);

			Sort(collection, compareFunc, firstIndex, currentIndex - 1);
			Sort(collection, compareFunc, currentIndex + 1, lastIndex);
		}

		public static void SortByDescending<T>(this IList<T> collection, Func<T, int> compareFunc, int firstIndex = 0, int lastIndex = int.MinValue)
		{
			if (lastIndex == int.MinValue)
			{
				lastIndex = collection.Count - 1;
			}
			if (firstIndex >= lastIndex)
			{
				return;
			}

			int middleIndex = (lastIndex - firstIndex) / 2 + firstIndex, currentIndex = firstIndex;

			collection.Swap(firstIndex, middleIndex);

			for (int i = firstIndex + 1; i <= lastIndex; ++i)
			{
				if (compareFunc((T)collection[i]) > compareFunc((T)collection[firstIndex]))
				{
					collection.Swap(++currentIndex, i);
				}
			}

			collection.Swap(firstIndex, currentIndex);

			SortByDescending(collection, compareFunc, firstIndex, currentIndex - 1);
			SortByDescending(collection, compareFunc, currentIndex + 1, lastIndex);
		}

		public static void SortByDescending<T>(this IList<T> collection, Func<T, float> compareFunc, int firstIndex = 0, int lastIndex = int.MinValue)
		{
			if (lastIndex == int.MinValue)
			{
				lastIndex = collection.Count - 1;
			}
			if (firstIndex >= lastIndex)
			{
				return;
			}

			int middleIndex = (lastIndex - firstIndex) / 2 + firstIndex, currentIndex = firstIndex;

			collection.Swap(firstIndex, middleIndex);

			for (int i = firstIndex + 1; i <= lastIndex; ++i)
			{
				if (compareFunc((T)collection[i]) > compareFunc((T)collection[firstIndex]))
				{
					collection.Swap(++currentIndex, i);
				}
			}

			collection.Swap(firstIndex, currentIndex);

			SortByDescending(collection, compareFunc, firstIndex, currentIndex - 1);
			SortByDescending(collection, compareFunc, currentIndex + 1, lastIndex);
		}

		public static List<T> ToList<T>(this IEnumerable<T> collection)
		{
			return new List<T>(collection);
		}

		public static List<R> Cast<T, R>(this IEnumerable<T> collection) where T : R
		{
			var result = new List<R>();

			foreach (var item in collection)
			{
				result.Add(item);
			}

			return result;
		}

		public static List<R> Cast<T, R>(this IEnumerable<T> collection, System.Func<T, R> convertFunc)
		{
			var result = new List<R>();

			foreach (var item in collection)
			{
				result.Add(convertFunc(item));
			}

			return result;
		}
	}
}