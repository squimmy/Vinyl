using Mono.Collections.Generic;

namespace Vinyl.Transformer
{
	public static class CollectionExtensions
	{
		public static void InsertRange<T>(this Collection<T> collection, int index, params T[] range)
		{
			foreach (var item in range)
			{
				collection.Insert(index, item);
				++index;
			}
		}
	}
}
