using D3DLab.Std.Engine.Common;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using D3DLab.Std.Engine.Core.Shaders;
using System.Runtime.InteropServices;
using D3DLab.Std.Engine.Shaders;
using D3DLab.Std.Engine.Core.Ext;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine.Components {
    /*
    public class SolidGeometryRenderComponent : ShaderComponent, IRenderableComponent {
        readonly Geometry3D geometry;

        DeviceBuffer indexBuffer;
        DeviceBuffer vertexBuffer;

        DeviceBuffer _worldBuffer;

        ResourceLayout projViewLayout;
        ResourceSet _projViewSet;

        public SolidGeometryRenderComponent(ShaderTechniquePass[] shaders, Geometry3D geometry) : base(shaders) {
            this.geometry = geometry;
        }

        public override VertexLayoutDescription[] GetLayoutDescription() {
            return new[] {
                new VertexLayoutDescription(
                        new VertexElementDescription("p", VertexElementSemantic.Position, VertexElementFormat.Float4),
                        new VertexElementDescription("n", VertexElementSemantic.Normal, VertexElementFormat.Float3),
                        new VertexElementDescription("c", VertexElementSemantic.Color, VertexElementFormat.Float4),
                        new VertexElementDescription("t", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
            };
        }

        public void Render(RenderState state) {
            var cmd = state.Commands;
            var factory = state.Factory;
            var gd = state.GrDevice;

            foreach (var pass in Passes) {
                var pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                        BlendStateDescription.SingleOverrideBlend,
                        DepthStencilStateDescription.Disabled,
                        RasterizerStateDescription.CullNone,
                        PrimitiveTopology.TriangleList,
                        pass.Description,
                        projViewLayout,
                        gd.SwapchainFramebuffer.OutputDescription));

                cmd.SetPipeline(pipeline);
                cmd.SetGraphicsResourceSet(0, _projViewSet);
                cmd.SetVertexBuffer(0, vertexBuffer);
                cmd.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
                cmd.DrawIndexed((uint)geometry.Indices.Count, 1, 0, 0, 0);
            }
        }

        public void Update(RenderState state) {
            var cmd = state.Commands;
            var factory = state.Factory;
            var gd = state.GrDevice;
            var viewport = state.Viewport;

            if (!Passes.All(x => x.IsCached)) {
                UpdateShaders(factory);
            }

            factory.CreateIfNullBuffer(ref _worldBuffer, new BufferDescription(64, BufferUsage.UniformBuffer));
            cmd.UpdateBuffer(_worldBuffer, 0, Matrix4x4.Identity);
            //
            var vertices = ConvertVertexToShaderStructure(geometry);
            factory.CreateIfNullBuffer(ref vertexBuffer, new BufferDescription((uint)(DefaultVertex.SizeInBytes * vertices.Length),
                BufferUsage.VertexBuffer));
            cmd.UpdateBuffer(vertexBuffer, 0, vertices);

            ushort[] indices = ConvertToShaderIndices(geometry);
            factory.CreateIfNullBuffer(ref indexBuffer, new BufferDescription(sizeof(ushort) * (uint)indices.Length,
                BufferUsage.IndexBuffer));
            cmd.UpdateBuffer(indexBuffer, 0, indices);

            if (projViewLayout == null) {
                projViewLayout = factory.CreateResourceLayout(
                  new ResourceLayoutDescription(
                      new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                      new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                      new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            }
            if (_projViewSet != null) {
                _projViewSet.Dispose();
            }
            _projViewSet = factory.CreateResourceSet(new ResourceSetDescription(
                       projViewLayout,
                       viewport.ProjectionBuffer,
                       viewport.ViewBuffer,
                       _worldBuffer));

        }

        DefaultVertex[] ConvertVertexToShaderStructure(Geometry3D geo) {
            var res = new List<DefaultVertex>();

            for (int i = 0; i < geo.Positions.Count; i++) {
                var pos = geo.Positions[i];
                res.Add(new DefaultVertex() {
                    Position = pos.ToVector4(),
                    Normal = geo.Normals[i],
                    Color = RgbaFloat.Red.ToVector4(),
                    //TexCoord = geo.TextureCoordinates
                });//
            }

            return res.ToArray();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DefaultVertex {
            public Vector4 Position;
            public Vector3 Normal;
            public Vector4 Color;
            public Vector2 TexCoord;

            public const int SizeInBytes = 4 * (0
                + 4 // Position
                + 3 // Normal
                + 4 // Color
                + 2 // TexCoord
                );
        }

    }*/

    public class LineGeometryRenderComponent : ShaderComponent, IRenderableComponent, IGeometryComponent {
        public Geometry3D Geometry { get; set; }
        
        public LineGeometryRenderComponent(IVeldridShaderSpecification shader, DeviceBufferesUpdater deviceBufferes, Geometry3D geometry) : base(shader, deviceBufferes) {
            this.Geometry = geometry;
        }

        public void Update(VeldridRenderState state) {
            var cmd = state.Commands;
            var factory = state.Factory;
            var viewport = state.Viewport;

            Bufferes.Update(factory, cmd);
            Resources.Update(factory, cmd);
            //          

            Shader.UpdateShaders(factory);

            Bufferes.UpdateWorld();
            Bufferes.UpdateVertex(Geometry);
            Bufferes.UpdateIndex(Geometry);

            Resources.UpdateResourceLayout();
            Resources.UpdateResourceSet(new ResourceSetDescription(
                       Resources.Layout,
                       viewport.ProjectionBuffer,
                       viewport.ViewBuffer,
                       Bufferes.World));
        }

        public void Render(VeldridRenderState state) {
            var cmd = state.Commands;
            var factory = state.Factory;
            var gd = state.GrDevice;
            var _pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                    BlendStateDescription.SingleOverrideBlend,
                    DepthStencilStateDescription.Disabled,
                    RasterizerStateDescription.CullNone,
                    PrimitiveTopology.LineList,
                    Shader.passes.First().Description,
                    new[] { Resources.Layout },
                    gd.SwapchainFramebuffer.OutputDescription));

            cmd.SetPipeline(_pipeline);
            cmd.SetGraphicsResourceSet(0, Resources.Set);
            cmd.SetVertexBuffer(0, Bufferes.Vertex);
            cmd.SetIndexBuffer(Bufferes.Index, IndexFormat.UInt16);
            cmd.DrawIndexed((uint)Geometry.Indices.Count, 1, 0, 0, 0);
        }


       
    }
}
