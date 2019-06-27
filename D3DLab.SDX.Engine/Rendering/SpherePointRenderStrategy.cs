using D3DLab.SDX.Engine.Components;
using D3DLab.SDX.Engine.Shader;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Filter;
using D3DLab.Std.Engine.Core.Shaders;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace D3DLab.SDX.Engine.Rendering {
    public class SpherePointRenderStrategy : RenderTechniqueSystem, IRenderTechniqueSystem {
        const string path = @"D3DLab.SDX.Engine.Rendering.Shaders.Custom.sphere_point.hlsl";

        static readonly D3DShaderTechniquePass pass;
        static readonly VertexLayoutConstructor layconst;

        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex {
            public readonly Vector3 Position;
            public readonly Vector4 Color;

            public Vertex(Vector3 position, Vector4 color) {
                Position = position;
                Color = color;
            }
            public static readonly int Size = Unsafe.SizeOf<Vertex>();
        }

        static SpherePointRenderStrategy() {
            layconst = new VertexLayoutConstructor()
                  .AddPositionElementAsVector3()
                  .AddColorElementAsVector4();

            var d = new CombinedShadersLoader();
            pass = new D3DShaderTechniquePass(d.Load(path, "SPH_"));
        }

        public IRenderTechniquePass GetPass() => pass;

        public SpherePointRenderStrategy() 
            : base(new EntityHasSet(
                typeof(D3DSpherePointRenderComponent),
                typeof(TransformComponent))) {
            rasterizerStateDescription = new RasterizerStateDescription() {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid,
                IsMultisampleEnabled = false,
                IsAntialiasedLineEnabled = false
            };
            depthStencilStateDescription = D3DDepthStencilStateDescriptions.DepthEnabled;
            blendStateDescription = D3DBlendStateDescriptions.BlendStateDisabled;
        }
        //

        protected override void Rendering(GraphicsDevice graphics, GameProperties game) {
            var device = graphics.D3DDevice;
            var context = graphics.ImmediateContext;

            foreach (var en in entities) {
                var render = en.GetComponent<D3DSpherePointRenderComponent>();
                var geo = en.GetComponent<IGeometryComponent>();
                var components = en.GetComponents<ID3DRenderable>();
                var transform = en.GetComponent<TransformComponent>();

                foreach (var com in components) {
                    if (com.IsModified) {
                        com.Update(graphics);
                    }
                }

                if (render.IsModified) {
                    if (!pass.IsCompiled) {
                        pass.Compile(graphics.Compilator);
                    }

                    UpdateShaders(graphics, render, pass, layconst);
                    render.PrimitiveTopology = PrimitiveTopology.PointList;;
                    render.IsModified = false;
                }

                UpdateTransformWorld(graphics, render, transform);

                SetShaders(context, render);

                context.VertexShader.SetConstantBuffer(GameStructBuffer.RegisterResourceSlot, game.Game);
                context.VertexShader.SetConstantBuffer(LightStructBuffer.RegisterResourceSlot, game.Lights);

                if (geo.IsModified) {
                    var center = geo.Positions[0];

                    var vertex = new Vertex[1];
                    vertex[0] = new Vertex(center, geo.Colors[0]);

                    render.VertexBuffer.Set(graphics.CreateBuffer(BindFlags.VertexBuffer, vertex));
                    render.IndexBuffer.Set(graphics.CreateBuffer(BindFlags.IndexBuffer, new[] { 0 }));

                    geo.MarkAsRendered();
                }

                Render(graphics, context, render, Vertex.Size);

                foreach (var com in components) {
                    com.Render(graphics);
                }

                graphics.ImmediateContext.Draw(geo.Positions.Length, 0);
            }
        }
    }
}
