using SvP.Engine.Common;
using SvP.Engine.Helpers;
using SvP.Engine.Systems;
using SvP.Standard.Engine.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;

namespace SvP.Engine {
    public static class Ex {
        public static void CreateIfNullBuffer(this DisposeCollectorResourceFactory f, ref DeviceBuffer b, BufferDescription desc) {
            b = b ?? f.CreateBuffer(desc);
        }
        public static void DoFirst<T>(this IEnumerable<T> enu, Action<T> action) where T : ID3DComponent {
            foreach (var i in enu) {
                action(i);
                break;
            }
        }
    }

    public class ViewportState {
        public Matrix4x4 WorldMatrix;
        /// <summary>
        /// Orthographic /Perspective 
        /// </summary>
        public Matrix4x4 ProjectionMatrix;
        /// <summary>
        /// the same as Camera
        /// </summary>
        public Matrix4x4 ViewMatrix;
    }

    public class RenderState {
        public ViewportState Viewport = new ViewportState();
        public float Ticks;
        public GraphicsDevice gd;
        public DisposeCollectorResourceFactory factory;
        public IAppWindow window;
        public CommandList Commands;
    }

    public interface IRenderableComponent : ID3DComponent {
        void Update(RenderState state);
        void Render(RenderState state);
    }

    public class TexturedGeometryGraphicsComponent : D3DComponent, IRenderableComponent {
        Shader[] shaders;
        readonly ShaderInfo[] shaderInfos;
        private DeviceBuffer _indexBuffer;
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _projectionBuffer;
        private DeviceBuffer _viewBuffer;
        ShaderSetDescription ShaderSet;

        public Matrix4x4 Matrix { get; set; }

        List<ResourceLayout> resourceLayouts = new List<ResourceLayout>();
        List<ResourceSet> ResourceSets = new List<ResourceSet>();
        readonly Geometry3D geometry;
        readonly string texture;

        public TexturedGeometryGraphicsComponent(ShaderInfo[] shaders, Geometry3D geometry, string texture) {
            this.texture = texture;
            this.geometry = geometry;
            this.shaderInfos = shaders;
            this.shaders = new Shader[0];
        }

        public void Update(RenderState state) {
            var _cl = state.Commands;
            var factory = state.factory;
            resourceLayouts.Clear();
            ResourceSets.Clear();

            if (!shaders.Any()) {
                shaders = shaderInfos.Select(x => ShaderHelper.LoadShader(factory, x.Path, x.Stage, x.EntryPoint)).ToArray();
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

            ImageSharpTexture stoneImage = new ImageSharpTexture(texture);
            var _surfaceTexture = stoneImage.CreateDeviceTexture(state.gd, factory);
            var _surfaceTextureView = factory.CreateTextureView(_surfaceTexture);

            ShaderSet = new ShaderSetDescription(new[] {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
            }, shaders);

            ResourceLayout projViewLayout = factory.CreateResourceLayout(
            new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            var _projViewSet = factory.CreateResourceSet(new ResourceSetDescription(
                 projViewLayout,
                 _projectionBuffer,
                 _viewBuffer));
            ResourceSets.Add(_projViewSet);
            resourceLayouts.Add(projViewLayout);

            ResourceLayout worldTextureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            resourceLayouts.Add(worldTextureLayout);

            var _worldTextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                  worldTextureLayout,
                  _worldBuffer,
                  _surfaceTextureView,
                  state.gd.Aniso4xSampler));

            ResourceSets.Add(_worldTextureSet);

        }

        public void Render(RenderState state) {
            var _cl = state.Commands;
            var factory = state.factory;
            var gd = state.gd;

            var _pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                    BlendStateDescription.SingleOverrideBlend,
                    DepthStencilStateDescription.DepthOnlyLessEqual,
                    RasterizerStateDescription.Default,
                    PrimitiveTopology.TriangleList,
                    ShaderSet,
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

        private VertexPositionTexture[] ConvertVertexToShaderStructure(Geometry3D geo) {
            var res = new List<VertexPositionTexture>();

            for (int i = 0; i < geo.Positions.Count; i++) {
              //  var index = geo.Indices[i];
                var pos = geo.Positions[i];
                var text = geo.TextureCoordinates[i];
                res.Add(new VertexPositionTexture(pos, text));
            }

            return res.ToArray();
        }
        public ushort[] ConvertToShaderIndices(Geometry3D geo) {
            return geo.Indices.Select(x => (ushort)x).ToArray();
        }


    }

    public class CameraInputSystem : IComponentSystem {
        public void Execute(SceneSnapshot snapshot) {

        }
    }
    public static class CameraBuilder {

        public class CameraComponent : D3DComponent, IRenderableComponent {
            private Vector3 _position;
            private float _moveSpeed = 10.0f;

            private float _yaw;
            private float _pitch;

            private Vector2 _previousMousePos;
            public float Width { get; set; }

            public float VWidth { get; set; }
            public float VHeight { get; set; }

            public Vector3 Position { get => _position; set { _position = value; UpdateViewMatrix(); } }
            public Vector3 LookDirection { get; private set; }
            public Vector3 UpDirection { get; private set; }
            public Matrix4x4 ViewMatrix { get; private set; }
            public Matrix4x4 ProjectionMatrix { get; private set; }
            public float FarDistance { get; }
            public float FieldOfView { get; }
            public float NearDistance { get; }
            public float AspectRatio => VWidth / VHeight;
            public float Yaw { get => _yaw; set { _yaw = value; UpdateViewMatrix(); } }
            public float Pitch { get => _pitch; set { _pitch = value; UpdateViewMatrix(); } }

            public CameraComponent(float width, float height) {
                _position = Vector3.UnitZ * 2.5f;
                FarDistance = 1000f;
                FieldOfView = 1f;
                NearDistance = 1f;

                UpDirection = Vector3.UnitY;
                LookDirection = new Vector3(0, -.3f, -1f);
                VWidth = width;
                Width = 3;
                VHeight = height;
                UpdatePerspectiveMatrix(width, height);
                UpdateViewMatrix();
            }

            public void WindowResized(float width, float height) {
                VWidth = width;
                VHeight = height;
                UpdatePerspectiveMatrix(width, height);
            }

            private void UpdatePerspectiveMatrix(float width, float height) {
                var aspectRatio = width / height;
                ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, width / height, NearDistance, FarDistance);

                float halfWidth = (float)(Width * 0.5f);
                float halfHeight = (float)((this.Width / aspectRatio) * 0.5f);

                ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-halfWidth, halfWidth, -halfHeight, halfHeight, NearDistance, FarDistance);
            }

            public void UpdatePerspectiveMatrix() {
                UpdatePerspectiveMatrix(VWidth, VHeight);
            }

            public void UpdateViewMatrix() {
                Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
                Vector3 lookDir = Vector3.Transform(-Vector3.UnitZ, lookRotation);
                LookDirection = lookDir;
                ViewMatrix = Matrix4x4.CreateLookAt(_position, _position + LookDirection, UpDirection);
            }
            public void Update(RenderState state) {
                var window = state.window;
                state.Viewport.ProjectionMatrix = ProjectionMatrix;
                state.Viewport.ViewMatrix = ViewMatrix;
                //angle += 1;

                //state.Viewport.ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                //    1.0f,
                //    (float)window.Width / window.Height,
                //    0.1f,
                //    1000f);
                //var pos = Vector3.UnitZ * 2.5f;
                //pos = Vector3.Transform(pos, Matrix4x4.CreateRotationY((float)((angle * Math.PI) / 180), Vector3.Zero));
                //state.Viewport.ViewMatrix = Matrix4x4.CreateLookAt(pos, Vector3.Zero, Vector3.UnitY);
            }

            public void Render(RenderState state) {

            }


        }

        public class GraphicsComponent : D3DComponent, IRenderableComponent {
            static float angle = 0;
            public void Update(RenderState state) {
                var window = state.window;

                return;


                angle += 1;

                state.Viewport.ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                    1.0f,
                    window.Width / window.Height,
                    0.1f,
                    1000f);
                var pos = Vector3.UnitZ * 2.5f;
                pos = Vector3.Transform(pos, Matrix4x4.CreateRotationY((float)((angle * Math.PI) / 180), Vector3.Zero));
                state.Viewport.ViewMatrix = Matrix4x4.CreateLookAt(pos, Vector3.Zero, Vector3.UnitY);
            }

            public void Render(RenderState state) {

            }
        }

    }

    public struct ShaderInfo {
        public string Path;
        public ShaderStages Stage;
        public string EntryPoint;
    }

    class GD {
        public static GraphicsDevice Create(IAppWindow window) {
            GraphicsDeviceOptions options = new GraphicsDeviceOptions(false, PixelFormat.R16_UNorm, true);
            return GraphicsDevice.CreateD3D11(options, window.Handle, (uint)window.Width, (uint)window.Height);
        }
    }

    public class Game {
        private static double _desiredFrameLengthSeconds = 1.0 / 60.0;

        readonly IAppWindow window;
        public readonly GraphicsDevice gd;
        public readonly DisposeCollectorResourceFactory factory;

        public ContextStateProcessor Context { get; }

        public Game(IAppWindow window, ContextStateProcessor context) {
            Context = context;
            this.gd = GD.Create(window);//for test
            this.window = window;
            this.factory = new DisposeCollectorResourceFactory(gd.ResourceFactory);
        }

        public void Run() {
            bool _limitFrameRate = true;
            long previousFrameTicks = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (window.IsActive) {
                long currentFrameTicks = sw.ElapsedTicks;
                double deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;

                while (_limitFrameRate && deltaSeconds < _desiredFrameLengthSeconds) {
                    currentFrameTicks = sw.ElapsedTicks;
                    deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;
                }

                //  Veldrid.InputSnapshot inputSnapshot = window.PumpEvents();
                window.ClearInput();
                try {

                    //var cmd = new Input.CameraZoomCommand(new Standard.Engine.Core.Input.InputEventState() {
                    //    Data = new Standard.Engine.Core.Input.InputStateData() {
                    //        CursorCurrentPosition = new Standard.Engine.Core.Input.WindowPoint(
                    //            (int)inputSnapshot.MousePosition.X,
                    //            (int)inputSnapshot.MousePosition.Y),
                    //        Delta = 3
                    //    }
                    //});

                    //foreach (var en in Context.GetEntityManager().GetEntities()) {
                    //    cmd.Execute(en);
                    //}

                    var snapshot = new SceneSnapshot(Context, null, TimeSpan.FromSeconds(deltaSeconds));
                    foreach (var sys in Context.GetSystemManager().GetSystems()) {
                        sys.Execute(snapshot);
                    }
                } catch (Exception ex) {
                    ex.ToString();
                    throw ex;
                }

            }

            gd.WaitForIdle();
            factory.DisposeCollector.DisposeAll();
            gd.Dispose();
        }
    }
}
