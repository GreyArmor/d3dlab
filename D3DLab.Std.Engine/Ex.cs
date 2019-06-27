using D3DLab.Std.Engine.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine {
    public static class Ex {
        public static void CreateIfNullBuffer(this DisposeCollectorResourceFactory f, ref DeviceBuffer b, BufferDescription desc) {
            b = b ?? f.CreateBuffer(desc);
        }
        public static void DoFirst<T>(this IEnumerable<T> enu, Action<T> action) where T : IGraphicComponent {
            foreach (var i in enu) {
                action(i);
                break;
            }
        }

        public static float SizeX(this BoundingBox bounds) {
            return Math.Abs(bounds.Min.X - bounds.Max.X);
        }
        public static float SizeY(this BoundingBox bounds) {
            return Math.Abs(bounds.Min.Y - bounds.Max.Y);
        }
        public static float SizeZ(this BoundingBox bounds) {
            return Math.Abs(bounds.Min.Z - bounds.Max.Z);
        }
    }
}
