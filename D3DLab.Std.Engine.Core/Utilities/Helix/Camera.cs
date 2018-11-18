﻿using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Ext;
using System;
using System.Numerics;

namespace D3DLab.Std.Engine.Core.Utilities.Helix {
    public enum CameraRotationMode {
        /// <summary>
        /// Turntable is constrained to two axes of rotation (model up and right direction)
        /// </summary>
        Turntable,

        /// <summary>
        /// Turnball using three axes (look direction, right direction and up direction (on the left/right edges)).
        /// </summary>
        Turnball,

        /// <summary>
        /// Using a virtual trackball.
        /// </summary>
        Trackball
    }
    public enum CameraMode {
        /// <summary>
        /// Orbits around a point (fixed target position, move closer target when zooming).
        /// </summary>
        Inspect,

        /// <summary>
        /// Walk around (fixed camera position when rotating, move in camera direction when zooming).
        /// </summary>
        WalkAround,

        /// <summary>
        /// Fixed camera target, change field of view when zooming.
        /// </summary>
        FixedPosition
    }

    public static class CameraMath {
        /// <summary>
        /// Rotates the trackball.
        /// </summary>
        /// <param name="cameraMode">The camera mode.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="rotateAround">The rotate around.</param>
        /// <param name="sensitivity">The sensitivity.</param>
        /// <param name="viewportWidth">Width of the viewport.</param>
        /// <param name="viewportHeight">Height of the viewport.</param>
        /// <param name="camera">The camera.</param>
        /// <param name="invertFactor">The invert factor. Right Handed System = 1; LeftHandedSystem = -1;</param>
        /// <param name="newPosition">The new position.</param>
        /// <param name="newLookDirection">The new look direction.</param>
        /// <param name="newUpDirection">The new up direction.</param>
        public static void RotateTrackball(CameraMode cameraMode, ref Vector2 p1, ref Vector2 p2, ref Vector3 rotateAround,
            float sensitivity,
            int viewportWidth, int viewportHeight, GeneralCameraComponent camera, int invertFactor,
            out Vector3 newPosition, out Vector3 newLookDirection, out Vector3 newUpDirection) {
            // http://viewport3d.com/trackball.htm
            // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
            var v1 = ProjectToTrackball(p1, viewportWidth, viewportHeight);
            var v2 = ProjectToTrackball(p2, viewportWidth, viewportHeight);
            var cUP = Vector3.Normalize(camera.UpDirection);
            // transform the trackball coordinates to view space
            var viewZ = Vector3.Normalize(camera.LookDirection * invertFactor);
            var viewX = Vector3.Cross(cUP, viewZ) * invertFactor;
            var viewY = Vector3.Cross(viewX, viewZ);
            var u1 = (viewZ * v1.Z) + (viewX * v1.X) + (viewY * v1.Y);
            var u2 = (viewZ * v2.Z) + (viewX * v2.X) + (viewY * v2.Y);

            // Could also use the Camera ViewMatrix
            // var vm = Viewport3DHelper.GetViewMatrix(this.ActualCamera);
            // vm.Invert();
            // var ct = new MatrixTransform3D(vm);
            // var u1 = ct.Transform(v1);
            // var u2 = ct.Transform(v2);

            // Find the rotation axis and angle
            var axis = Vector3.Cross(u1, u2);
            if (axis.LengthSquared() < 1e-8) {
                newPosition = camera.Position;
                newLookDirection = camera.LookDirection;
                newUpDirection = camera.UpDirection;
                return;
            }

            var angle = u1.AngleRad(u2);

            // Create the transform
            var rotate = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(axis), -angle * sensitivity * 5);

            // Find vectors relative to the rotate-around point
            var relativeTarget = rotateAround - camera.Target;
            var relativePosition = rotateAround - camera.Position;

            // Rotate the relative vectors
            var newRelativeTarget = Vector3.Transform(relativeTarget, rotate);
            var newRelativePosition = Vector3.Transform(relativePosition, rotate);
            newUpDirection = Vector3.TransformNormal(cUP, rotate);

            // Find new camera position
            var newTarget = rotateAround - newRelativeTarget;
            newPosition = rotateAround - newRelativePosition;

            newLookDirection = newTarget - newPosition;
            if (cameraMode != CameraMode.Inspect) {
                newPosition = camera.Position;
            }
        }

        /// <summary>
        /// Projects a screen position to the trackball unit sphere.
        /// </summary>
        /// <param name="point">
        /// The screen position.
        /// </param>
        /// <param name="w">
        /// The width of the viewport.
        /// </param>
        /// <param name="h">
        /// The height of the viewport.
        /// </param>
        /// <returns>
        /// A trackball coordinate.
        /// </returns>
        private static Vector3 ProjectToTrackball(Vector2 point, double w, double h) {
            // Use the diagonal for scaling, making sure that the whole client area is inside the trackball
            double r = Math.Sqrt((w * w) + (h * h)) / 2;
            double x = (point.X - (w / 2)) / r;
            double y = ((h / 2) - point.Y) / r;
            double z2 = 1 - (x * x) - (y * y);
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Rotates around three axes.
        /// </summary>
        /// <param name="cameraMode">The camera mode.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="rotateAround">The rotate around.</param>
        /// <param name="sensitivity">The sensitivity.</param>
        /// <param name="viewportWidth">Width of the viewport.</param>
        /// <param name="viewportHeight">Height of the viewport.</param>
        /// <param name="camera">The camera.</param>
        /// <param name="invertFactor">The invert factor. Right Handed = 1; Left Handed = -1</param>
        /// <param name="newPosition">The new position.</param>
        /// <param name="newLookDirection">The new look direction.</param>
        /// <param name="newUpDirection">The new up direction.</param>
        public static void RotateTurnball(CameraMode cameraMode, ref Vector2 p1, ref Vector2 p2, ref Vector3 rotateAround,
            float sensitivity,
            int viewportWidth, int viewportHeight,
            GeneralCameraComponent camera, int invertFactor,
            out Vector3 newPosition, out Vector3 newLookDirection, out Vector3 newUpDirection) {
            InitTurnballRotationAxes(p1, viewportWidth, viewportHeight, camera, out var rotationAxisX, out var rotationAxisY);

            Vector2 delta = p2 - p1;

            var relativeTarget = rotateAround - camera.Target;
            var relativePosition = rotateAround - camera.Position;

            float d = -1;
            if (cameraMode != CameraMode.Inspect) {
                d = 0.2f;
            }

            d *= sensitivity;

            var q1 = Quaternion.CreateFromAxisAngle(rotationAxisX, d * invertFactor * delta.X / 180 * (float)Math.PI);
            var q2 = Quaternion.CreateFromAxisAngle(rotationAxisY, d * delta.Y / 180 * (float)Math.PI);
            Quaternion q = q1 * q2;

            var m = Matrix4x4.CreateFromQuaternion(q);
            Vector3 newLookDir = Vector3.TransformNormal(Vector3.Normalize(camera.LookDirection), m);
            newUpDirection = Vector3.TransformNormal(Vector3.Normalize(camera.UpDirection), m);

            Vector3 newRelativeTarget = Vector3.Transform(relativeTarget, m);
            Vector3 newRelativePosition = Vector3.Transform(relativePosition, m);

            var newRightVector = Vector3.Cross(newLookDir, newUpDirection);
            var modUpDir = Vector3.Cross(newRightVector, newLookDir);
            if ((newUpDirection - modUpDir).Length() > 1e-8) {
                newUpDirection = modUpDir;
            }

            var newTarget = rotateAround - newRelativeTarget;
            newPosition = rotateAround - newRelativePosition;
            newLookDirection = newTarget - newPosition;

            if (cameraMode != CameraMode.Inspect) {
                newPosition = camera.Position;
            }
        }

        /// <summary>
        /// Initializes the 'turn-ball' rotation axes from the specified point.
        /// </summary>
        /// <param name="p1">
        /// The point.
        /// </param>
        /// <param name="camera"></param>
        /// <param name="rotationAxisX"></param>
        /// <param name="rotationAxisY"></param>
        /// <param name="viewportHeight"></param>
        /// <param name="viewportWidth"></param>
        public static void InitTurnballRotationAxes(Vector2 p1, int viewportWidth, int viewportHeight,
            GeneralCameraComponent camera,
            out Vector3 rotationAxisX, out Vector3 rotationAxisY) {
            double fx = p1.X / viewportWidth;
            double fy = p1.Y / viewportHeight;

            var up = Vector3.Normalize(camera.UpDirection);
            var dir = Vector3.Normalize(camera.LookDirection);

            var right = Vector3.Normalize(Vector3.Cross(dir, up));

            rotationAxisX = up;
            rotationAxisY = right;

            if (fx > 0.8) {
                // delta.X = 0;
                rotationAxisY = dir;
            }

            if (fx < 0.2) {
                // delta.X = 0;
                rotationAxisY = -dir;
            }
        }

        /// <summary>
        /// Rotates using turntable.
        /// </summary>
        /// <param name="cameraMode">The camera mode.</param>
        /// <param name="delta">The delta.</param>
        /// <param name="rotateAround">The rotate around.</param>
        /// <param name="sensitivity">The sensitivity.</param>
        /// <param name="viewportWidth">Width of the viewport.</param>
        /// <param name="viewportHeight">Height of the viewport.</param>
        /// <param name="camera">The camera.</param>
        /// <param name="invertFactor">The invert factor. Right Handed = 1; Left Handed = -1;</param>
        /// <param name="modelUpDirection">The model up direction.</param>
        /// <param name="newPosition">The new position.</param>
        /// <param name="newLookDirection">The new look direction.</param>
        /// <param name="newUpDirection">The new up direction.</param>
        public static void RotateTurntable(CameraMode cameraMode, ref Vector2 delta, ref Vector3 rotateAround,
            float sensitivity,
            int viewportWidth, int viewportHeight,
            GeneralCameraComponent camera, int invertFactor,
            Vector3 modelUpDirection,
            out Vector3 newPosition, out Vector3 newLookDirection, out Vector3 newUpDirection) {

            var relativeTarget = rotateAround - camera.Target;
            var relativePosition = rotateAround - camera.Position;
            var cUp = Vector3.Normalize(camera.UpDirection);
            var up = modelUpDirection;
            var dir = Vector3.Normalize(camera.LookDirection);
            var right = Vector3.Cross(dir, cUp);

            float d = -0.5f;
            if (cameraMode != CameraMode.Inspect) {
                d *= -0.2f;
            }

            d *= sensitivity;

           // up = Vector3.UnitY;
            //right = Vector3.UnitX;

            System.Diagnostics.Trace.WriteLine($"UP:{up} Right: {right}, delta: {delta}" );

            

            var q1 = Quaternion.CreateFromAxisAngle(up, d * invertFactor * delta.X / 180 * (float)Math.PI);
            var q2 = Quaternion.CreateFromAxisAngle(right, d * delta.Y / 180 * (float)Math.PI);
            Quaternion q = q1 * q2;

            var m = Matrix4x4.CreateFromQuaternion(q);

            newUpDirection = Vector3.TransformNormal(cUp, m);

            var newRelativeTarget = Vector3.Transform(relativeTarget, m);
            var newRelativePosition = Vector3.Transform(relativePosition, m);

            var newTarget = rotateAround - newRelativeTarget;
            newPosition = rotateAround - newRelativePosition;

            newLookDirection = (newTarget - newPosition);
            if (cameraMode != CameraMode.Inspect) {
                newPosition = camera.Position;
            }
        }
    }
}
