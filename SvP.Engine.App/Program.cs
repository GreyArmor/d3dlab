using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using D3DLab.Std.Engine.Common;
using D3DLab.Std.Engine.Helpers;
using D3DLab.Std.Engine.Systems;
using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;
using D3DLab.Std.Engine.Core.Shaders;

namespace D3DLab.Std.Engine.App {
    public class Sdl2WindowWrapper : IAppWindow {
        readonly Sdl2Window w;

        public Sdl2WindowWrapper(Sdl2Window w) {
            this.w = w;
        }

        public float Width => w.Width;
        public float Height => w.Height;
        public bool IsActive => w.Exists;

        public IntPtr Handle => w.Handle;

        public Core.Input.InputSnapshot GetShapshot() {
            w.PumpEvents();
            return null;
        }
    }

    class Program {
        public sealed class EngineNotificator : IEngineSubscribe, IManagerChangeNotify, IEntityRenderNotify {
            private readonly List<IEngineSubscriber> subscribers;
            public EngineNotificator() {
                this.subscribers = new List<IEngineSubscriber>();
            }

            public void Subscribe(IEngineSubscriber s) {
                subscribers.Add(s);
            }

            public void NotifyChange<T>(T _object) where T : class {
                var handlers = subscribers.OfType<IManagerChangeSubscriber<T>>();
                foreach (var handler in handlers) {
                    handler.Change(_object);
                }
            }
            public void NotifyRender(IEnumerable<GraphicEntity> entities) {
                var handlers = subscribers.OfType<IEntityRenderSubscriber>();
                foreach (var handler in handlers) {
                    handler.Render(entities);
                }
            }
        }
        public sealed class GenneralContextState : BaseContextState {
            public GenneralContextState(ContextStateProcessor processor) : base(processor) {
            }
        }

        private static Sdl2WindowWrapper window;

        static void Main(string[] args) {

            WindowCreateInfo windowCI = new WindowCreateInfo {
                X = 50,
                Y = 50,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowInitialState = WindowState.Normal,
                WindowTitle = "Veldrid NeoDemo"
            };

            Sdl2Window w = VeldridStartup.CreateWindow(windowCI);
            window = new Sdl2WindowWrapper(w);

            var context = new ContextStateProcessor(new EngineNotificator());
            context.AddState(0, x => new GenneralContextState(x));
            context.SwitchTo(0);

            var game = new Game(window, context);

            //====

            D3DShaderInfo[] shaders = {
                    new D3DShaderInfo{ Path= Path.Combine(AppContext.BaseDirectory, "Shaders", "Cube"),
                        Stage = ShaderStages.Vertex.ToString(), EntryPoint = "VS" },
                    new D3DShaderInfo{ Path= Path.Combine(AppContext.BaseDirectory, "Shaders","Cube"),
                        Stage = ShaderStages.Fragment.ToString(), EntryPoint = "FS"}
                };

            var mb = new Helpers.MeshBulder();

            var camera = context.GetEntityManager()
                .CreateEntity(new ElementTag(Guid.NewGuid().ToString()))
                .AddComponent(new CameraBuilder.CameraComponent(window.Width, window.Height));
            // .AddComponent(new CameraBuilder.GraphicsComponent());

            var mage = Image.Load(Path.Combine(AppContext.BaseDirectory, "Textures", "spnza_bricks_a_diff.png"));

            var image = new TextureInfo() {
                Path = Path.Combine(AppContext.BaseDirectory, "Textures", "spnza_bricks_a_diff.png"),
                ///Image = Image.Load(Path.Combine(AppContext.BaseDirectory, "Textures", "spnza_bricks_a_diff.png"))
            };

            var box = GetCubeVertice();
            box = mb.BuildBox(Vector3.Zero, 1, 1, 1);

            var geo = context.GetEntityManager()
                .CreateEntity(new ElementTag(Guid.NewGuid().ToString()))
                .AddComponent(new TexturedGeometryGraphicsComponent(shaders, box, image) {
                    Matrix = Matrix4x4.CreateTranslation(Vector3.UnitX * 1)
                });

            var geo1 = context.GetEntityManager()
                .CreateEntity(new ElementTag(Guid.NewGuid().ToString()))
                .AddComponent(new TexturedGeometryGraphicsComponent(shaders, mb.BuildSphere(Vector3.Zero, 1), image) { Matrix = Matrix4x4.CreateTranslation(Vector3.UnitY * -1) });

            context.GetSystemManager()
                .CreateSystem<VeldridRenderSystem>()
                .Init(game.gd, game.factory, window);

            context.EntityOrder
                .RegisterOrder<VeldridRenderSystem>(camera.Tag, 0)
                .RegisterOrder<VeldridRenderSystem>(geo.Tag, 1)
                .RegisterOrder<VeldridRenderSystem>(geo1.Tag, 2);

            //====


            game.Run(new EngineNotificator());
        }


        public static Shader LoadShader(ResourceFactory factory, string set, ShaderStages stage, string entryPoint) {
            string pathNoExtention = Path.Combine(AppContext.BaseDirectory, "Shaders", set);
            return ShaderHelper.LoadShader(factory, pathNoExtention, stage, entryPoint);
        }
       
        public static VertexPositionTexture[] GetCubeVertices1() {
            VertexPositionTexture[] vertices = new[]
            {
                // Top
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 1)),
                // Bottom                                                             
                new VertexPositionTexture(new Vector3(-0.5f,-0.5f, +0.5f),  new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f,-0.5f, +0.5f),  new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+0.5f,-0.5f, -0.5f),  new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f,-0.5f, -0.5f),  new Vector2(0, 1)),
                // Left                                                               
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                // Right                                                              
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
                // Back                                                               
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                // Front                                                              
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
            };

            return vertices;
        }

        public static Geometry3D GetCubeVertice() {
            var cube = GetCubeVertices1();
            var box = new Geometry3D() {
                Indices = GetCubeIndices1().Select(x => (int)x).ToList(),
                Positions = cube.Select(x => x.Position).ToList(),
                TextureCoordinates = cube.Select(x => x.TextureCoordinates).ToList()
            };
            return box;
        }

        public static ushort[] GetCubeIndices1() {
            ushort[] indices =
            {
                0,1,2, 0,2,3,
                4,5,6, 4,6,7,
                8,9,10, 8,10,11,
                12,13,14, 12,14,15,
                16,17,18, 16,18,19,
                20,21,22, 20,22,23,
            };

            return indices;
        }
    }
}
