using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace D3DLab.Debugger {
    public static class NumericsTypes {
        static readonly Guid vec3 = typeof(Vector3).GUID;
        public static bool IsVector3(Type t) {
            return t.GUID == vec3;
        }
    }
}
