using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using D3DLab.SDX.Engine.Components;
using D3DLab.SDX.Engine.Rendering.Strategies;
using D3DLab.SDX.Engine.Shader;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Filter;
using D3DLab.Std.Engine.Core.Shaders;
using SharpDX.Direct3D11;

namespace D3DLab.SDX.Engine.Rendering {
    public class LineVertexRenderTechnique : RenderTechniqueSystem, IRenderTechniqueSystem {
        const string path = @"D3DLab.SDX.Engine.Rendering.Shaders.Custom.Lines.hlsl";

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

        static LineVertexRenderTechnique() {
            layconst = new VertexLayoutConstructor()
               .AddPositionElementAsVector3()
               .AddColorElementAsVector4();

            var d = new CombinedShadersLoader();
            pass = new D3DShaderTechniquePass(d.Load(path, "LV_"));
        }

        public IRenderTechniquePass GetPass() => pass;

        public LineVertexRenderTechnique()
            : base(new EntityHasSet(
                typeof(D3DLineVertexRenderComponent),
                typeof(TransformComponent))) {
            rasterizerStateDescription = new RasterizerStateDescription() {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid,
                IsMultisampleEnabled = false,
                IsAntialiasedLineEnabled = true
            };
            depthStencilStateDescription = D3DDepthStencilStateDescriptions.DepthEnabled;
            blendStateDescription = D3DBlendStateDescriptions.BlendStateDisabled;
        }

        protected override void Rendering(GraphicsDevice graphics, GameProperties game) {
            var device = graphics.D3DDevice;
            var context = graphics.ImmediateContext;

            foreach (var en in entities) {
                var render = en.GetComponent<D3DLineVertexRenderComponent>();
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
                    render.IsModified = false;
                }

                UpdateTransformWorld(graphics, render, transform);

                SetShaders(context, render);

                context.VertexShader.SetConstantBuffer(GameStructBuffer.RegisterResourceSlot, game.Game);
                context.VertexShader.SetConstantBuffer(LightStructBuffer.RegisterResourceSlot, game.Lights);

                if (geo.IsModified) {
                    var pos = geo.Positions;

                    var vertex = new Vertex[pos.Length];
                    for (var i = 0; i < pos.Length; i++) {
                        vertex[i] = new Vertex(pos[i], geo.Colors[i]);
                    }

                    render.VertexBuffer.Set(graphics.CreateBuffer(BindFlags.VertexBuffer, vertex));
                    render.IndexBuffer.Set(graphics.CreateBuffer(BindFlags.IndexBuffer, geo.Indices.ToArray()));

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
