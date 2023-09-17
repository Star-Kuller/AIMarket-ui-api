using System.Collections.Generic;
using System.Linq;

namespace IAE.Microservice.Application.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                from acc in accumulator
                from item in sequence
                select acc.Concat(new[] { item }));
        }

        public static IEnumerable<IList<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> source)
        {
            var enumerators = source.Select(e => e.GetEnumerator()).ToArray();
            try
            {
                while (enumerators.All(e => e.MoveNext()))
                {
                    yield return enumerators.Select(e => e.Current).ToArray();
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                    enumerator.Dispose();
            }
        }
    }
}
