using D3DLab.Std.Engine.Common;
using D3DLab.Std.Engine.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;
using D3DLab.Std.Engine.Core.Shaders;
using System.IO;
using System;
using D3DLab.Std.Engine.Shaders;

namespace D3DLab.Std.Engine.Components {
    public struct TextureInfo {
        public string Path;
    }
    /*
    public class TexturedGeometryRenderComponent : ShaderComponent, IRenderableComponent {
        private DeviceBuffer _indexBuffer;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _projectionBuffer;
        private DeviceBuffer _viewBuffer;
        ResourceLayout projViewLayout;
        ResourceLayout worldTextureLayout;
        TextureView _surfaceTextureView;

        public Matrix4x4 Matrix { get; set; }

        List<ResourceLayout> resourceLayouts = new List<ResourceLayout>();
        List<ResourceSet> ResourceSets = new List<ResourceSet>();
        readonly Geometry3D geometry;
        readonly TextureInfo texture;

        public TexturedGeometryRenderComponent(ShaderTechniquePass[] shaders, Geometry3D geometry, TextureInfo texture) : base(shaders) {
            this.texture = texture;
            this.geometry = geometry;
        }

        public override VertexLayoutDescription[] GetLayoutDescription() {
            return new[] {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
                };
        }

        public void Update(RenderState state) {
            var _cl = state.Commands;
            var factory = state.Factory;
            resourceLayouts.Clear();
            ResourceSets.Clear();

            //if (!TechniquePass.IsCached) {
            //    TechniquePass.Update(factory, ShaderInfos);
            //    ShaderSet = new ShaderSetDescription(GetLayoutDescription(), TechniquePass.ToArray());
            //}
            if (!Passes.Any(x=>x.IsCached)) {
                UpdateShaders(factory);
            }

            factory.CreateIfNullBuffer(ref _projectionBuffer, new BufferDescription(64, BufferUsage.UniformBuffer));
            factory.CreateIfNullBuffer(ref _viewBuffer, new BufferDescription(64, BufferUsage.UniformBuffer));

            _cl.UpdateBuffer(_projectionBuffer, 0, state.Viewport.ProjectionMatrix);
            _cl.UpdateBuffer(_viewBuffer, 0, state.Viewport.ViewMatrix);

            VertexPositionTexture[] vertices = ConvertVertexToShaderStructure(geometry);//GetCubeVertices();
            factory.CreateIfNullBuffer(ref _vertexBuffer, new BufferDescription((uint)(VertexPositionTexture.SizeInBytes * vertices.Length),
                BufferUsage.VertexBuffer));
            _cl.UpdateBuffer(_vertexBuffer, 0, vertices);

            ushort[] indices = ConvertToShaderIndices(geometry);//GetCubeIndices();
            factory.CreateIfNullBuffer(ref _indexBuffer, new BufferDescription(sizeof(ushort) * (uint)indices.Length,
                BufferUsage.IndexBuffer));
            _cl.UpdateBuffer(_indexBuffer, 0, indices);

            //---- 
            var _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            Matrix4x4 rotation = Matrix;//Matrix4x4.CreateTranslation(Vector3.UnitX * 1);
                                        //Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, (state.Ticks / 1000f)) *
                                        //Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, (state.Ticks / 3000f));

            _cl.UpdateBuffer(_worldBuffer, 0, rotation);

            if (_surfaceTextureView == null) {
                ImageSharpTexture stoneImage = new ImageSharpTexture(texture.Path);
                var _surfaceTexture = stoneImage.CreateDeviceTexture(state.GrDevice, factory);
                _surfaceTextureView = factory.CreateTextureView(_surfaceTexture);
            }
            if (projViewLayout == null) {
                projViewLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex)));
            }
            if (worldTextureLayout == null) {
                worldTextureLayout = factory.CreateResourceLayout(
                    new ResourceLayoutDescription(
                        new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                        new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                        new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
            }


            var _projViewSet = factory.CreateResourceSet(new ResourceSetDescription(
                 projViewLayout,
                 _projectionBuffer,
                 _viewBuffer));
            ResourceSets.Add(_projViewSet);

            resourceLayouts.Add(projViewLayout);
            resourceLayouts.Add(worldTextureLayout);

            var _worldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                  worldTextureLayout,
                  _worldBuffer,
                  _surfaceTextureView,
                  state.GrDevice.Aniso4xSampler));

            ResourceSets.Add(_worldTextureSet);

        }

        public void Render(RenderState state) {
            var _cl = state.Commands;
            var factory = state.Factory;
            var gd = state.GrDevice;

            var _pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                    BlendStateDescription.SingleOverrideBlend,
                    DepthStencilStateDescription.DepthOnlyLessEqual,
                    RasterizerStateDescription.Default,
                    PrimitiveTopology.TriangleList,
                    Passes.First().Description,
                    resourceLayouts.ToArray(),
                    gd.SwapchainFramebuffer.OutputDescription));
            _cl.SetPipeline(_pipeline);

            uint index = 0;
            for (; ResourceSets.Count > index; ++index) {
                _cl.SetGraphicsResourceSet(index, ResourceSets[(int)index]);
            }

            _cl.SetVertexBuffer(0, _vertexBuffer);
            _cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _cl.DrawIndexed((uint)geometry.Indices.Count, 1, 0, 0, 0);
        }

        VertexPositionTexture[] ConvertVertexToShaderStructure(Geometry3D geo) {
            var res = new List<VertexPositionTexture>();

            for (int i = 0; i < geo.Positions.Count; i++) {
                //  var index = geo.Indices[i];
                var pos = geo.Positions[i];
                var text = geo.TextureCoordinates[i];
                res.Add(new VertexPositionTexture(pos, text));
            }

            return res.ToArray();
        }
        ushort[] ConvertToShaderIndices(Geometry3D geo) {
            return geo.Indices.Select(x => (ushort)x).ToArray();
        }


    }*/
}
