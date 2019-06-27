using SvP.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SvP.Engine.Helpers {
    public class MeshBulder {
        [Flags]
        private enum BoxFaces {
            /// <summary>
            /// The top.
            /// </summary>
            PositiveZ = 0x1,

            /// <summary>
            /// The bottom.
            /// </summary>
            NegativeZ = 0x2,

            /// <summary>
            /// The left side.
            /// </summary>
            NegativeY = 0x4,

            /// <summary>
            /// The right side.
            /// </summary>
            PositiveY = 0x8,

            /// <summary>
            /// The front side.
            /// </summary>
            PositiveX = 0x10,

            /// <summary>
            /// The back side.
            /// </summary>
            NegativeX = 0x20,

            /// <summary>
            /// All sides.
            /// </summary>
            All = PositiveZ | NegativeZ | NegativeY | PositiveY | PositiveX | NegativeX
        }

        public Geometry3D BuildBox(Vector3 center, double xlength, double ylength, double zlength) {
            var geo = new Geometry3D();
            BoxFaces faces = BoxFaces.All;

            if ((faces & BoxFaces.PositiveZ) == BoxFaces.PositiveZ) {
                this.AddCubeFace(geo, center, new Vector3(0, 0, 1), new Vector3(0, 1, 0), zlength, xlength, ylength);
            }

            if ((faces & BoxFaces.NegativeZ) == BoxFaces.NegativeZ) {
                this.AddCubeFace(geo, center, new Vector3(0, 0, -1), new Vector3(0, 1, 0), zlength, xlength, ylength);
            }

            if ((faces & BoxFaces.PositiveX) == BoxFaces.PositiveX) {
                this.AddCubeFace(geo, center, new Vector3(1, 0, 0), new Vector3(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.NegativeX) == BoxFaces.NegativeX) {
                this.AddCubeFace(geo, center, new Vector3(-1, 0, 0), new Vector3(0, 0, 1), xlength, ylength, zlength);
            }

            if ((faces & BoxFaces.NegativeY) == BoxFaces.NegativeY) {
                this.AddCubeFace(geo, center, new Vector3(0, -1, 0), new Vector3(0, 0, 1), ylength, xlength, zlength);
            }

            if ((faces & BoxFaces.PositiveY) == BoxFaces.PositiveY) {
                this.AddCubeFace(geo, center, new Vector3(0, 1, 0), new Vector3(0, 0, 1), ylength, xlength, zlength);
            }

           

            return geo;
        }
        private void AddCubeFace(Geometry3D geo, Vector3 center, Vector3 normal, Vector3 up, double depth, double width, double height) {
            var right = Vector3.Cross(normal, up);
            var n = normal * (float)depth / 2;
            up *= (float)height / 2f;
            right *= (float)width / 2f;
            var p1 = center + n - up - right;
            var p2 = center + n - up + right;
            var p3 = center + n + up + right;
            var p4 = center + n + up - right;

            var i0 = geo.Positions.Count;
            geo.Positions.Add(p1);
            geo.Positions.Add(p2);
            geo.Positions.Add(p3);
            geo.Positions.Add(p4);

            if (geo.Normals != null) {
                geo.Normals.Add(normal);
                geo.Normals.Add(normal);
                geo.Normals.Add(normal);
                geo.Normals.Add(normal);
            }

            if (geo.TextureCoordinates != null) {
                geo.TextureCoordinates.Add(new Vector2(1, 1));
                geo.TextureCoordinates.Add(new Vector2(0, 1));
                geo.TextureCoordinates.Add(new Vector2(0, 0));
                geo.TextureCoordinates.Add(new Vector2(1, 0));
            }

            geo.Indices.Add(i0 + 0);
            geo.Indices.Add(i0 + 1);
            geo.Indices.Add(i0 + 2);

            geo.Indices.Add(i0 + 0);
            geo.Indices.Add(i0 + 2);
            geo.Indices.Add(i0 + 3);

            //geo.Indices.Add(i0 + 2);
            //geo.Indices.Add(i0 + 1);
            //geo.Indices.Add(i0 + 0);

            //geo.Indices.Add(i0 + 0);
            //geo.Indices.Add(i0 + 3);
            //geo.Indices.Add(i0 + 2);
        }

        public Geometry3D BuildSphere(Vector3 center, double radius = 1, int thetaDiv = 32, int phiDiv = 32) {
            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var textureCoordinates = new List<Vector2>();
            var triangleIndices = new List<int>();

            var index0 = positions.Count;
            var dt = (float)(2 * Math.PI / thetaDiv);
            var dp = (float)(Math.PI / phiDiv);

            for (var pi = 0; pi <= phiDiv; pi++) {
                var phi = pi * dp;

                for (var ti = 0; ti <= thetaDiv; ti++) {
                    // we want to start the mesh on the x axis
                    var theta = ti * dt;

                    // Spherical coordinates
                    // http://mathworld.wolfram.com/SphericalCoordinates.html
                    //float x = (float)(Math.Cos(theta) * Math.Sin(phi));
                    //float y = (float)(Math.Sin(theta) * Math.Sin(phi));
                    //float z = (float)(Math.Cos(phi));

                    var x = (float)(Math.Sin(theta) * Math.Sin(phi));
                    var y = (float)(Math.Cos(phi));
                    var z = (float)(Math.Cos(theta) * Math.Sin(phi));

                    var p = new Vector3(center.X + ((float)radius * x), center.Y + ((float)radius * y), center.Z + ((float)radius * z));
                    positions.Add(p);

                    if (normals != null) {
                        var n = new Vector3(x, y, z);
                        normals.Add(n);
                    }

                    if (textureCoordinates != null) {
                        var uv = new Vector2((float)(theta / (2 * Math.PI)), (float)(phi / Math.PI));
                        textureCoordinates.Add(uv);
                    }
                }
            }

            this.AddRectangularMeshTriangleIndices(triangleIndices, index0, phiDiv + 1, thetaDiv + 1, true);

            return new Geometry3D() {
                Positions = positions.ToList(),
                Indices = triangleIndices.ToList(),
                Normals = normals.ToList(),
                TextureCoordinates = textureCoordinates.ToList()
            };
        }

        private void AddRectangularMeshTriangleIndices(List<int> triangleIndices, int index0, int rows, int columns, bool isSpherical = false) {
            for (int i = 0; i < rows - 1; i++) {
                for (int j = 0; j < columns - 1; j++) {
                    int ij = (i * columns) + j;
                    if (!isSpherical || i > 0) {
                        triangleIndices.Add(index0 + ij);
                        triangleIndices.Add(index0 + ij + 1 + columns);
                        triangleIndices.Add(index0 + ij + 1);
                    }

                    if (!isSpherical || i < rows - 2) {
                        triangleIndices.Add(index0 + ij + 1 + columns);
                        triangleIndices.Add(index0 + ij);
                        triangleIndices.Add(index0 + ij + columns);
                    }
                }
            }
        }
    }
}
