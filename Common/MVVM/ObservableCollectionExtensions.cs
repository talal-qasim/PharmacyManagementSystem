using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Common.MVVM
{
    /// <summary>
    /// Extension methods for converting query/DAL results into ObservableCollection&lt;T&gt;,
    /// per rulebook Rule 6.4. Every List ViewModel calls this after a DAL "Get[Entities]()" call.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return new ObservableCollection<T>();
            }

            return new ObservableCollection<T>(source.ToList());
        }
    }
}
