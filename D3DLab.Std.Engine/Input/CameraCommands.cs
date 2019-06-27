using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Ext;
using D3DLab.Std.Engine.Core.Input;
using D3DLab.Std.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine.Input {
    public class CameraZoomCommand : IInputCommand {
        const float scrollSpeed = 0.5f;
        readonly InputStateData state;

        public CameraZoomCommand(InputStateData state) {
            this.state = state;
        }

        public bool Execute(GraphicEntity entity) {
            var find = entity.GetComponents<VeldridCameraBuilder.VeldridCameraComponent>();
            if (!find.Any()) {
                return false;
            }
            var ccom = find.First();
            var delta = state.Delta;

            var nscale = ccom.Scale + (delta * 0.001f);
            if (nscale > 0) {
                ccom.Scale = nscale;
            }

            return true;
        }
    }

    public class CameraRotateCommand : IInputCommand {
        readonly InputStateData state;

        public CameraRotateCommand(InputStateData state) {
            this.state = state;
        }

        public bool Execute(GraphicEntity entity) {
            var find = entity.GetComponents<VeldridCameraBuilder.VeldridCameraComponent>();
            if (!find.Any()) {
                return false;
            }
            var ccom = find.First();

            var p11 = state.ButtonsStates[GeneralMouseButtons.Right].PointDown;
            var p2 = state.CurrentPosition;

            var moveV = p2 - p11;
            if (moveV == Vector2.Zero) {
                return false;
            }
            var v2Up = new Vector2(0, -1);
            var mouseMove = moveV;
            var angleLook = v2Up.AngleRad(mouseMove.Normalize());

            //Console.WriteLine($"Angle 2D: {v2Up.Angle(mouseMove.Normalize())}");

            var look = ccom.LookDirection.Normalize();
            var up = ccom.UpDirection.Normalize();

            var rotatedUp = Vector3.TransformNormal(up, Matrix4x4.CreateFromAxisAngle(look, angleLook));
            var cross = Vector3.Cross(look, rotatedUp);

            var angle = mouseMove.Length();

            var movetozero = Matrix4x4.CreateTranslation(ccom.RotatePoint * -1f);
            var rotate = Matrix4x4.CreateFromAxisAngle(cross, angle.ToRad());
            var returntocenter = Matrix4x4.CreateTranslation(ccom.RotatePoint);
            var matrixRotate = movetozero * rotate * returntocenter;

            if (matrixRotate.IsIdentity) {
                return false;
            }

            ccom.UpDirection = Vector3.TransformNormal(ccom.UpDirection.Normalize(), matrixRotate).Normalize();
            ccom.LookDirection = Vector3.TransformNormal(ccom.LookDirection.Normalize(), matrixRotate).Normalize();
            ccom.Position = Vector3.Transform(ccom.Position, matrixRotate);

            return true;
        }
    }

    public class ZoomToObjectCommand : IInputCommand {
        readonly BoundingBox box;

        public ZoomToObjectCommand(BoundingBox box) {
            this.box = box;
        }

        public bool Execute(GraphicEntity entity) {
            var find = entity.GetComponents<VeldridCameraBuilder.VeldridCameraComponent>();
            if (!find.Any()) {
                return false;
            }
            var com = find.First();

            var x = box.SizeX();
            var y = box.SizeY();
            var ratio = x / y;
            var max = Math.Max(box.SizeX(), box.SizeY()) * 1.2f;

            com.Position = box.GetCenter() + Vector3.UnitZ * max;
            com.RotatePoint = box.GetCenter();
            com.Width = max;
            com.LookDirection = new Vector3(0, 0, -3);
            com.UpDirection = new Vector3(0, 1, 0);
            com.Scale = 1;
            // com.LookDirection = -Vector3.UnitZ * max;

            //ccom.Width *= box.SizeX() / viewport.ActualWidth;
            //var oldTarget = pcam.Position + pcam.LookDirection;
            //var distance = pcam.LookDirection.Length;
            //var newTarget = centerRay.PlaneIntersection(oldTarget, w);
            //if (newTarget != null) {
            //    orthographicCamera.LookDirection = w * distance;
            //    orthographicCamera.Position = newTarget.Value - orthographicCamera.LookDirection;
            //}

            return true;
        }
    }
}
