using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Ext {
    public static class EnumerableEx {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (var item in source) {
                action(item);
            }
        }
    }
}
