using SvP.Standard.Engine.Core;
using SvP.Standard.Engine.Core.Ext;
using SvP.Standard.Engine.Core.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SvP.Engine.Input {
    class CameraZoomCommand : IInputCommand {
        readonly InputEventState istate;

        public CameraZoomCommand(InputEventState state) {
            this.istate = state;
        }

        public void Execute(Entity entity) {
            var find = entity.GetComponents<CameraBuilder.CameraComponent>();
            if (!find.Any()) {
                return;
            }
            
            var ccom = find.First();
            var state = istate.Data;

            var delta = state.Delta;

            var PanK = 1;

            var x = state.CursorCurrentPosition.X;
            var y = state.CursorCurrentPosition.Y;

            var kx = x / ccom.VWidth;
            var ky = y / ccom.VHeight;

            var p1 = new Vector2(x * PanK, y * PanK);
            var p0 = new Vector2(ccom.VWidth * 0.5f * PanK, ccom.VHeight * 0.5f * PanK);

            var d = 1 - delta * 0.001f;
            var prevWidth = ccom.Width;

            var newWidth = ccom.Width * d;
            d = newWidth / prevWidth;

            var pan = (p1 - p0) * (d - 1);

            var left = Vector3.Cross(ccom.UpDirection.Normalize(), ccom.LookDirection.Normalize());
            left.Normalize();

            var panVector = left * pan.X + ccom.UpDirection.Normalize() * pan.Y;

            ccom.Width = newWidth;
            ccom.Position = ccom.Position + panVector;

            ccom.UpdatePerspectiveMatrix();
            ccom.UpdateViewMatrix();
        }
    }
}
