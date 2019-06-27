using D3DLab.Std.Engine.Common;
using D3DLab.Std.Engine.Components;
using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Shaders;
using D3DLab.Std.Engine.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine.Render {
    public class BoundingBoxRenderableComponent : ShaderComponent, IRenderableComponent {

        public BoundingBox Box { get;}

        readonly Geometry3D geometry;

        public BoundingBoxRenderableComponent(IVeldridShaderSpecification shader, DeviceBufferesUpdater deviceBufferes, BoundingBox box) 
            : base(shader, deviceBufferes) {
            EntityTag = new ElementTag("BoundingBox");
            Box = box;
            geometry = new Geometry3D();
            var corners = box.GetCorners();

            var points = new List<Vector3>() {
                corners.NearBottomLeft,
                corners.NearBottomRight,
                corners.NearTopLeft,
                corners.NearTopRight,
                corners.FarBottomLeft,
                corners.FarBottomRight,
                corners.FarTopLeft,
                corners.FarTopRight
            };

            geometry.Positions = points;
            var index = 0;
            geometry.Indices = new List<int>(geometry.Positions.Select(x => index++));

            var lb = new Std.Engine.Helpers.LineBuilder();
            geometry = lb.Build(points);
        }

        public override void Dispose() {

        }

        public void Update(VeldridRenderState state) {
            var cmd = state.Commands;
            var factory = state.Factory;
            var viewport = state.Viewport;

            Bufferes.Update(factory, cmd);
            Resources.Update(factory, cmd);
            
            Shader.UpdateShaders(factory);

            Bufferes.UpdateWorld();
            Bufferes.UpdateVertex(geometry);
            Bufferes.UpdateIndex(geometry);

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
            cmd.DrawIndexed((uint)geometry.Indices.Count, 1, 0, 0, 0);
            //cmd.Draw((uint)geometry.Positions.Count);
        }
    }
}
