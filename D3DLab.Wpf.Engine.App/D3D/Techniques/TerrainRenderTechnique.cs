using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using D3DLab.SDX.Engine;
using D3DLab.SDX.Engine.Components;
using D3DLab.SDX.Engine.D2;
using D3DLab.SDX.Engine.Rendering;
using D3DLab.SDX.Engine.Rendering.Strategies;
using D3DLab.SDX.Engine.Shader;
using D3DLab.Std.Engine.Core.Common;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Materials;
using D3DLab.Std.Engine.Core.Filter;
using D3DLab.Std.Engine.Core.Shaders;
using D3DLab.Wpf.Engine.App.D3D.Components;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace D3DLab.Wpf.Engine.App.D3D.Techniques {
    class TerrainRenderTechnique : RenderTechniqueSystem, IRenderTechniqueSystem {
        const string path = @"D3DLab.Wpf.Engine.App.D3D.Shaders.terrain.hlsl";

        static readonly D3DShaderTechniquePass pass;
        static readonly VertexLayoutConstructor layconst;

        static TerrainRenderTechnique() {
            layconst = new VertexLayoutConstructor()
               .AddPositionElementAsVector3()
               .AddNormalElementAsVector3()
               .AddTexCoorElementAsVector2()
               .AddTexCoorElementAsVector2()
               .AddTangentElementAsVector3()
               .AddBinormalElementAsVector3()
               .AddColorElementAsVector4();             

            var d = new CombinedShadersLoader(typeof(TerrainRenderTechnique));

            pass = new D3DShaderTechniquePass(d.Load(path, "TRR_"));
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TerrainVertex {
            internal Vector3 position;
            internal Vector3 normal;
            internal Vector2 texcoor;
            internal Vector2 normapMapTexCoor;
            internal Vector3 tangent;
            internal Vector3 binormal;
            internal Vector4 color;
            

            public static readonly int Size = Unsafe.SizeOf<TerrainVertex>();
        }

        public TerrainRenderTechnique()
            : base(new EntityHasSet(
                typeof(D3DTerrainRenderComponent), 
                typeof(D3DTexturedMaterialSamplerComponent))
                  ) {
            rasterizerStateDescription = new RasterizerStateDescription() {
                CullMode = CullMode.Front,
                FillMode = FillMode.Solid,
                IsMultisampleEnabled = false,
                IsAntialiasedLineEnabled = false
            };
            // disable depth because SKY dom is close to camera and with correct depth overlap all objects
            depthStencilStateDescription = D3DDepthStencilStateDescriptions.DepthEnabled;
            blendStateDescription = D3DBlendStateDescriptions.BlendStateDisabled;
        }

        public IRenderTechniquePass GetPass() => pass;

        protected override void Rendering(GraphicsDevice graphics, GameProperties game) {
            var device = graphics.D3DDevice;
            var context = graphics.ImmediateContext;

            foreach (var en in entities) {
                var render = en.GetComponent<D3DTerrainRenderComponent>();
                var geo = en.GetComponent<TerrainGeometryCellsComponent>();
                var material = en.GetComponent<D3DTexturedMaterialSamplerComponent>();
                var transform = en.GetComponent<TransformComponent>();
                var components = en.GetComponents<ID3DRenderable>();

                foreach (var com in components) {
                    if (com.IsModified) {
                        com.Update(graphics);
                    }
                }

                if (render.IsModified || !pass.IsCompiled) {//update
                    if (!pass.IsCompiled) {
                        pass.Compile(graphics.Compilator);
                    }

                    UpdateShaders(graphics, render, pass, layconst);
                    render.PrimitiveTopology = PrimitiveTopology.TriangleList;

                    render.IsModified = false;
                }

                UpdateTransformWorld(graphics, render, transform);
                SetShaders(context, render);
                
                if (material.IsModified) {
                    render.TextureResources.Set(ConvertToResources(material, graphics.TexturedLoader));
                    render.SampleState.Set(graphics.CreateSampler(material.SampleDescription));
                    material.IsModified = false;
                }
                //
                
                context.VertexShader.SetConstantBuffer(GameStructBuffer.RegisterResourceSlot, game.Game);
                context.VertexShader.SetConstantBuffer(LightStructBuffer.RegisterResourceSlot, game.Lights);

                //context.PixelShader.SetConstantBuffer(GameStructBuffer.RegisterResourceSlot, game.Game);
                //context.PixelShader.SetConstantBuffer(LightStructBuffer.RegisterResourceSlot, game.Lights);

                context.PixelShader.SetConstantBuffer(GameStructBuffer.RegisterResourceSlot, game.Game);
                context.PixelShader.SetConstantBuffer(LightStructBuffer.RegisterResourceSlot, game.Lights);
                context.PixelShader.SetShaderResources(0, render.TextureResources.Get());
                context.PixelShader.SetSampler(0, render.SampleState.Get());

                if (geo.IsModified) {
                    UpdateCellBuffers(graphics, geo, render);

                    /*
                     * <remark>https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1?view=netcore-2.2<remark>
                     */
                    /*
                   var pool = ArrayPool<TerrainVertex>.Shared;
                   TerrainVertex[] vertex = null;
                   try {
                       var pos = geo.Positions;
                       var normals = geo.Normals;
                       //vertex = new TerrainVertex[pos.Length];
                       vertex = pool.Rent(pos.Length);//
                       for (var i = 0; i < pos.Length; i++) {
                           vertex[i] = new TerrainVertex {
                               position = pos[i],
                               normal = normals[i],
                               tangent = geo.Tangents[i],
                               binormal = geo.Binormal[i],
                               color = geo.Colors[i],
                               texcoor = geo.TextureCoordinates[i],
                               normapMapTexCoor = geo.NormalMapTexCoordinates[i],
                           };
                       }

                       render.VertexBuffer.Set(graphics.CreateBuffer(BindFlags.VertexBuffer, vertex));
                       render.IndexBuffer.Set(graphics.CreateBuffer(BindFlags.IndexBuffer, geo.Indices.ToArray()));

                       geo.MarkAsRendered();
                   } finally {
                      pool.Return(vertex);
                   }
                   */
                   geo.MarkAsRendered();
                }

                RenderCells(graphics, game.CameraState.GetFrustum(), geo, render, transform);

                //Frustum frustum = null;
                //for (var i = 0; i < render.CellCount; ++i) {
                //    var cell = render.GetCell(0, frustum);

                //    Render(graphics, context, render, TerrainVertex.Size);

                //    foreach (var com in components) {
                //        com.Render(graphics);
                //    }

                //    graphics.ImmediateContext.DrawIndexed(geo.Indices.Length, 0, 0);
                ////}
            }


            
        }


        void UpdateCellBuffers(GraphicsDevice graphics, 
            TerrainGeometryCellsComponent geo, D3DTerrainRenderComponent render) {

            var vbuff = new SharpDX.Direct3D11.Buffer[geo.Cells.Length];
            var ibuff = new SharpDX.Direct3D11.Buffer[geo.Cells.Length];

            for (var cellIndex = 0; cellIndex < geo.Cells.Length; cellIndex++) {
                var cell = geo.Cells[cellIndex];
                var pool = ArrayPool<TerrainVertex>.Shared;
                TerrainVertex[] vertex = null;
                try {
                    //vertex = new TerrainVertex[pos.Length];
                    vertex = pool.Rent(cell.VertexCount);//
                    for (var i = 0; i < cell.VertexCount; i++) {
                        vertex[i] = new TerrainVertex {
                            position = cell.Positions[i],
                            normal = cell.Normals[i],
                            tangent = cell.Tangents[i],
                            binormal = cell.Binormal[i],
                            color = cell.Colors[i],
                            texcoor = cell.TextureCoordinates[i],
                            normapMapTexCoor = cell.NormalMapTexCoordinates[i],
                        };
                    }

                    vbuff[cellIndex] = graphics.CreateBuffer(BindFlags.VertexBuffer, vertex);
                    ibuff[cellIndex] = graphics.CreateBuffer(BindFlags.IndexBuffer, cell.Indices);

                } finally {
                    pool.Return(vertex);
                }
            }
            render.VertexBuffers.Set(vbuff);
            render.IndexBuffers.Set(ibuff);
        }

        void RenderCells(GraphicsDevice graphics, Frustum frustum,
            TerrainGeometryCellsComponent geo, D3DTerrainRenderComponent render, TransformComponent transform) {
            var context = graphics.ImmediateContext;
            if (render.TransformWorldBuffer.HasValue) {
                context.VertexShader.SetConstantBuffer(TransforStructBuffer.RegisterResourceSlot, render.TransformWorldBuffer.Get());
            }

            context.InputAssembler.InputLayout = render.Layout.Get();
            context.InputAssembler.PrimitiveTopology = render.PrimitiveTopology;
            graphics.UpdateRasterizerState(render.RasterizerState.GetDescription());

            context.OutputMerger.SetDepthStencilState(render.DepthStencilState.Get(), 0);
            var blendFactor = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0);
            context.OutputMerger.SetBlendState(render.BlendingState.Get(), blendFactor, -1);


            for(var index =0; index < geo.Cells.Length; ++index) {

                var cell = geo.Cells[index];
                var box = cell.Tree.GetBounds().Transform(transform.MatrixWorld);

                if (cell.Tree.IsBuilt && frustum.Contains(ref box) != Frustum.ContainmentType.Disjoint) {
                    context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(render.VertexBuffers.Get()[index], TerrainVertex.Size, 0));
                    context.InputAssembler.SetIndexBuffer(render.IndexBuffers.Get()[index], SharpDX.DXGI.Format.R32_UInt, 0);

                    graphics.ImmediateContext.DrawIndexed(cell.IndexCount, 0, 0);
                }
              
            }
        }
    }
}
