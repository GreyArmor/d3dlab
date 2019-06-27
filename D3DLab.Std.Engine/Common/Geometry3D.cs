using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine.Common {
    public sealed class Geometry3D : D3DLab.Std.Engine.Core.Common.AbstractGeometry3D {

        public BoundingBox Bounds {
            get {
                if(bounds.GetDimensions() == Vector3.Zero) {
                    bounds = FromPoints(Positions.ToArray());
                }
                return bounds;
            }
        }
        BoundingBox bounds;

        public Geometry3D() {
        }


        static BoundingBox FromPoints(Vector3[] points) {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            for (int i = 0; i < points.Length; ++i) {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            return new BoundingBox(min, max);
        }
    }
}
