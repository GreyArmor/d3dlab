using D3DLab.Std.Engine;
using D3DLab.Std.Engine.Common;
using D3DLab.Std.Engine.Components;
using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Shaders;
using D3DLab.Std.Engine.Shaders;
using D3DLab.Std.Engine.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace D3DLab.Wpf.Engine.App {
    public static class SolidGeometryEntityBuilder {
        public static void Build(IContextState context, Geometry3D geo) {
            IShaderInfo[] shaders = {
                    new D3DShaderInfo(
                        Path.Combine(AppContext.BaseDirectory, "Shaders", "SolidMesh"),
                        $"solid-{ShaderStages.Vertex}",
                        ShaderStages.Vertex.ToString(),
                        "VShaderDefault"),
                        //new ShaderInfo{ Path= $"{Path.Combine(AppContext.BaseDirectory, "Shaders", "Line", "line")}-{ShaderStages.Geometry}.hlsl",
                    //    Stage = ShaderStages.Geometry.ToString(), EntryPoint = "GShaderLines"},
                     new D3DShaderInfo(
                        Path.Combine(AppContext.BaseDirectory, "Shaders", "SolidMesh"),
                        $"solid-{ShaderStages.Fragment}",
                        ShaderStages.Fragment.ToString(),
                        "FS")
                };
            var line = context.GetEntityManager()
                  .CreateEntity(new ElementTag(Guid.NewGuid().ToString()))
                  ;//.AddComponent(new SolidGeometryRenderComponent(new[] { new ShaderTechniquePass(shaders) }, geo));

            context.EntityOrder.RegisterOrder<VeldridRenderSystem>(line.Tag);
        }
    }

    public static class LineEntityBuilder {
        public class LineShaderSpecification : ShaderSpecification<LineShaderSpecification.LinesVertex> {
            public LineShaderSpecification(ShaderTechniquePass[] passes) : base(passes) { }

            public override VertexLayoutDescription[] GetVertexDescription() {
                return new[] {
                    new VertexLayoutDescription(
                            new VertexElementDescription("p", VertexElementSemantic.Position, VertexElementFormat.Float4),
                            new VertexElementDescription("c", VertexElementSemantic.Color, VertexElementFormat.Float4))
                };
            }
            public override ResourceLayoutDescription GetResourceDescription() {
                return new ResourceLayoutDescription(
                       new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                       new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                       new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex));
            }
            public override uint GetVertexSizeInBytes() {
                return LinesVertex.SizeInBytes;
            }

            protected override LinesVertex[] ConvertVertexToShaderStructure(Geometry3D geo) {
                var res = new List<LinesVertex>();

                for (int i = 0; i < geo.Positions.Count; i++) {
                    var pos = geo.Positions[i];
                    res.Add(new LinesVertex() { Position = new Vector4(pos.X, pos.Y, pos.Z, 1), Color = RgbaFloat.Red.ToVector4() });
                }

                return res.ToArray();
            }

            [StructLayout(LayoutKind.Sequential, Pack = 4)]
            public struct LinesVertex {
                public Vector4 Position;
                public Vector4 Color;
                public const int SizeInBytes = 4 * (4 + 4);
            }
        }
        public class LineDeviceBufferesUpdater : DeviceBufferesUpdater {
            public LineDeviceBufferesUpdater(LineShaderSpecification shader) : base(shader) {

            }
            public override void UpdateVertex(Geometry3D geo) {
                var vertices = shader.ConvertVertexToShaderStructure<LineShaderSpecification.LinesVertex>(geo);
                factory.CreateIfNullBuffer(ref vertexBuffer, new BufferDescription((uint)(shader.GetVertexSizeInBytes() * vertices.Length),
                   BufferUsage.VertexBuffer));
                cmd.UpdateBuffer(vertexBuffer, 0, vertices);
            }
        }

        public static void Build(IContextState context, Vector3[] points) {
            IShaderInfo[] lineShaders = {
                    new D3DShaderInfo(
                        Path.Combine(AppContext.BaseDirectory, "Shaders", "Line"),
                        $"line-{ShaderStages.Vertex}",
                        ShaderStages.Vertex.ToString(),
                        "VShaderLines"),
                        //new ShaderInfo{ Path= $"{Path.Combine(AppContext.BaseDirectory, "Shaders", "Line", "line")}-{ShaderStages.Geometry}.hlsl",
                    //    Stage = ShaderStages.Geometry.ToString(), EntryPoint = "GShaderLines"},
                     new D3DShaderInfo(
                        Path.Combine(AppContext.BaseDirectory, "Shaders", "Line"),
                        $"line-{ShaderStages.Fragment}",
                        ShaderStages.Fragment.ToString(),
                        "PShaderLinesFade")
                };

            var mb = new Std.Engine.Helpers.MeshBulder();
            //var box = mb.BuildBox(Vector3.Zero, 1, 1, 1);

            var lb = new Std.Engine.Helpers.LineBuilder();

            //var box = lb.Build(mb.BuildBox(Vector3.Zero, -20, 20, 20).Positions);// points.ToList().GetRange(0,20));
            var dd = points.ToList().GetRange(0, 5).ToList();
            var box = lb.Build(points);
            var shader = new LineShaderSpecification(new[] { new ShaderTechniquePass(lineShaders) });
            //
            var line = context.GetEntityManager()
                  .CreateEntity(new ElementTag("LineEntity"))
                  .AddComponent(new LineGeometryRenderComponent(
                      shader,
                      new LineDeviceBufferesUpdater(shader),
                      box
                  // new Std.Engine.Helpers.LineBuilder().Build(box.Positions.GetRange(0,3))
                  ));

            context.EntityOrder.RegisterOrder<VeldridRenderSystem>(line.Tag);
        }
    }
}
