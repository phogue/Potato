using System.Collections.Generic;

namespace Procon.Nlp.Utils {
    public static class ListExtensions {

        public static void ReplaceRange<T>(this List<T> list, int start, int count, List<T> replacement) where T : class {
            list.RemoveRange(start, count);
            list.InsertRange(start, replacement);
        }

        public static void ReplaceRange<T>(this List<T> list, int start, int count, T replacement) where T : class {
            list.RemoveRange(start, count);

            if (replacement != null) {
                list.Insert(start, replacement);
            }
        }
    }
}
