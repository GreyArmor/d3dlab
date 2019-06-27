using System;
using System.Collections.Generic;

namespace D3DLab.Std.Engine.Core.Ext {
    public static class ArrayGeneratorEx {
        public static uint[] ToUIntArray(this int length) {
            var res = new uint[length];
            for(uint i =0; i < length; ++i) {
                res[i] = i;
            }
            return res;
        }

        public static T[] SelectToArray<T>(this int count, Func<T> each) {
            var res = new T[count];
            for (uint i = 0; i < count; ++i) {
                res[i] = each();
            }
            return res;
        }
        public static List<T> SelectToList<T>(this int count, Func<T> each) {
            var res = new List<T>(count);
            for (uint i = 0; i < count; ++i) {
                res.Add(each());
            }
            return res;
        }
    }
}
