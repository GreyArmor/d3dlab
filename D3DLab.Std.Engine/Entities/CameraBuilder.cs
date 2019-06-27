using D3DLab.Std.Engine.Components;
using D3DLab.Std.Engine.Core;
using System;
using System.Numerics;
using Veldrid;

namespace D3DLab.Std.Engine.Entities {
    public static class VeldridCameraBuilder {

        public class VeldridCameraComponent : Core.Components.CameraComponent, IRenderableComponent {
           
            public VeldridCameraComponent(float width, float height):base(width,height) {
               
            }
            
            #region render

            public void Update(VeldridRenderState state) {
                var factory = state.Factory;
                var cmd = state.Commands;
                var window = state.Window;

                UpdateViewMatrix();
                UpdatePerspectiveMatrix(window.Width, window.Height);

                state.Viewport.ProjectionMatrix = ProjectionMatrix;
                state.Viewport.ViewMatrix = ViewMatrix;

                factory.CreateIfNullBuffer(ref state.Viewport.ProjectionBuffer, new BufferDescription(64, BufferUsage.UniformBuffer));
                factory.CreateIfNullBuffer(ref state.Viewport.ViewBuffer, new BufferDescription(64, BufferUsage.UniformBuffer));

                cmd.UpdateBuffer(state.Viewport.ProjectionBuffer, 0, state.Viewport.ProjectionMatrix);
                cmd.UpdateBuffer(state.Viewport.ViewBuffer, 0, state.Viewport.ViewMatrix);
            }

            public void Render(VeldridRenderState state) {

            }


            #endregion

            public override string ToString() {
                return $"Pos:{Position}; LoockDirection:{LookDirection}; Scale:{Scale}";
            }

        }

    }
}
