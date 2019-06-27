using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using D3DLab.Std.Engine.Core.Components;
using g3;

namespace D3DLab.Std.Engine.Core.Utilities {
    public static class Readers {
        public static HittableGeometryComponent ReadObj(FileInfo file) {

            var reader = new OBJReader();
            var builder = new SimpleMeshBuilder();
            using (var text = new StreamReader(File.OpenRead(file.FullName))) {
                var result = reader.Read(text, ReadOptions.Defaults, builder);
            }

            
            
            //var reader = new StandardMeshReader();
            //var res = reader.Read(file.FullName, ReadOptions.Defaults);

            //StandardMeshReader.ReadMesh()

            var mesh = builder.Meshes[0];

            

            return new HittableGeometryComponent {
                Indices = mesh.GetTriangleArray().ToImmutableArray(),
                Normals = mesh.NormalsItr().Select(n => new Vector3(n.x, n.y, n.z)).ToImmutableArray(),
                Positions = mesh.VerticesItr().Select(n => new Vector3((float)n.x, (float)n.y, (float)n.z)).ToImmutableArray(),

            };
        }
    }

    public static class Writter {
        public static void WriteObj(FileInfo file, List<Vector3> points ) {
            using (var str = new StreamWriter(File.OpenWrite(file.FullName))) {
                for (var i = 0; i < points.Count; ++i) {
                    str.WriteLine($"v {points[i].X} {points[i].Y} {points[i].Z}");
                }
                //for (var i = 0; i < tecture.Count; ++i) {
                //    str.WriteLine($"vt {tecture[i].X} {tecture[i].Y}");
                //}
                //for (var i = 0; i < normals.Count; ++i) {
                //    str.WriteLine($"vn {normals[i].X} {normals[i].Y} {normals[i].Z}");
                //}
                //for (var i = 0; i < indx.Count - 3; i += 3) {
                //    str.WriteLine($"f {indx[i]} {indx[i + 1]} {indx[i + 2]}");
                //}
            }
        }

    }

}
