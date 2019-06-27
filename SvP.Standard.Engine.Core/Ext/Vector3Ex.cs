using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace D3DLab.Std.Standard.Engine.Core.Ext {
    public static class Vector3Ex {
        public static Vector3 Normalize(this Vector3 v) {
            return Vector3.Normalize(v);
        }
        public static Vector3 Cross(this Vector3 v1, Vector3 v2) {
            return Vector3.Cross(v1, v2);
        }
    }
}
