using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using g3;

namespace D3DLab.Std.Engine.Core.Ext {
    public static class g3Ex {
        public static Vector3f ToVector3f(this Vector3 v) {
            return new Vector3f(v.X, v.Y, v.Z);
        }
        public static Vector3 ToVector3(this Vector3f v) {
            return new Vector3(v.x, v.y, v.z);
        }
        public static Vector3 ToVector3(this Vector3d v) {
            return new Vector3((float)v.x, (float)v.y, (float)v.z);
        }
    }
}
