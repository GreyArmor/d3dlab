using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SvP.Engine.Common {
    public sealed class Geometry3D {
        public List<Vector3> Positions { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Vector2> TextureCoordinates { get; set; }
        public List<int> Indices { get; set; }

        public Geometry3D() {
            Positions = new List<Vector3>();
            Normals = new List<Vector3>();
            TextureCoordinates = new List<Vector2>();
            Indices = new List<int>();
        }
    }
}
