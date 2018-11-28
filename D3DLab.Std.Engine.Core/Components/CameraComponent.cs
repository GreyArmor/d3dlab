﻿using D3DLab.Std.Engine.Core.Ext;
using System.Numerics;

namespace D3DLab.Std.Engine.Core.Components {
    public struct CameraState {
        public Vector3 UpDirection;
        public Vector3 LookDirection;
        public Vector3 Position;
        public Vector3 Target;

        public Matrix4x4 ProjectionMatrix;
        public Matrix4x4 ViewMatrix;
    }

    public class PerspectiveCameraComponent : GeneralCameraComponent {

        public float FieldOfViewRadians { get; set; }
        public float MinimumFieldOfView { get; set; }
        public float MaximumFieldOfView { get; set; }

        public PerspectiveCameraComponent() {
            ResetToDefault();
        }

        public override Matrix4x4 UpdateProjectionMatrix(float width, float height) {
            float aspectRatio = width / height;

            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                        FieldOfViewRadians,
                        aspectRatio,
                        NearPlaneDistance,
                        FarPlaneDistance);
            
            return ProjectionMatrix;
        }

        public override void ResetToDefault() {
            UpDirection = Vector3.UnitY;
            FieldOfViewRadians = 1.05f;
            NearPlaneDistance = 100f;
            LookDirection = ForwardRH;
            Position = Vector3.UnitZ * 200f;

            FarPlaneDistance = Position.Length() * 70;
        }
    }

    public class OrthographicCameraComponent : GeneralCameraComponent {
        public float Width { get; set; }

        protected float scale;
        protected float prevScreenWidth;
        protected float prevScreenHeight;

        public OrthographicCameraComponent(float width, float height) {
            this.prevScreenWidth = width;
            this.prevScreenHeight = height;
            scale = 1;
            ResetToDefault();
        }

        public override Matrix4x4 UpdateProjectionMatrix(float width, float height) {
            this.prevScreenWidth = width;
            this.prevScreenHeight = height;
            float aspectRatio = width / height;

            var frameWidth = Width * scale;
            ProjectionMatrix = Matrix4x4.CreateOrthographic(
                        frameWidth,
                        frameWidth / aspectRatio,
                        NearPlaneDistance,
                        FarPlaneDistance);
            return ProjectionMatrix;
        }

        public override void ResetToDefault() {
            UpDirection = Vector3.UnitY;
            Width = 35f;
            //FieldOfViewRadians = 1.05f;
            NearPlaneDistance = 0.01f;
            LookDirection = ForwardRH;
            Position = Vector3.UnitZ * Width * 10f;

            FarPlaneDistance = Position.Length() * 50;

            scale = 1;
        }

        public void Pan(Vector2 move) {
            var PanK = (Width * scale) / prevScreenWidth;
            var p1 = new Vector2(move.X * PanK, move.Y * PanK);

            var left = Vector3.Cross(UpDirection, LookDirection);
            left.Normalized();

            var panVector = left * p1.X + UpDirection * p1.Y;
            Position += panVector;
        }

        static Vector3 Project(Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, Matrix4x4 worldViewProjection) {
            var v = Vector3.Transform(vector, worldViewProjection);
            return new Vector3(((1.0f + v.X) * 0.5f * width) + x, ((1.0f - v.Y) * 0.5f * height) + y, (v.Z * (maxZ - minZ)) + minZ);
        }

        static Vector3 Unproject(Vector3 vector, float x, float y, float width, float height, float minZ, float maxZ, Matrix4x4 worldViewProjection) {
            Vector3 v = new Vector3();
            Matrix4x4 matrix = new Matrix4x4();
            Matrix4x4.Invert(worldViewProjection, out matrix);

            v.X = (((vector.X - x) / width) * 2.0f) - 1.0f;
            v.Y = -((((vector.Y - y) / height) * 2.0f) - 1.0f);
            v.Z = (vector.Z - minZ) / (maxZ - minZ);

            return Vector3.Transform(v, matrix);
        }

        public Veldrid.Utilities.Ray GetRayFromScreenPoint(Vector2 screen) {
            var screenX = screen.X;
            var screenY = screen.Y;
            var scaleFactor = new Vector2(1, 1);
            // Normalized Device Coordinates Top-Left (-1, 1) to Bottom-Right (1, -1)
            float x = (2.0f * screenX) / (prevScreenWidth / scaleFactor.X) - 1.0f;
            float y = 1.0f - (2.0f * screenY) / (prevScreenHeight / scaleFactor.Y);
            float z = 1.0f;
            Vector3 deviceCoords = new Vector3(x, y, z);

            // Clip Coordinates
            Vector4 clipCoords = new Vector4(deviceCoords.X, deviceCoords.Y, -1.0f, 1.0f);

            // View Coordinates
            Matrix4x4.Invert(ProjectionMatrix, out Matrix4x4 invProj);
            Vector4 viewCoords = Vector4.Transform(clipCoords, invProj);
            viewCoords.Z = -1.0f;
            viewCoords.W = 0.0f;

            Matrix4x4.Invert(ViewMatrix, out Matrix4x4 invView);
            Vector3 worldCoords = Vector4.Transform(viewCoords, invView).XYZ();
            worldCoords = Vector3.Normalize(worldCoords);

            return new Veldrid.Utilities.Ray(Position, worldCoords);
        }
    }

    public abstract class GeneralCameraComponent : GraphicComponent {
        protected bool COORDINATE_SYSTEM_LH = false;

        // A unit Vector3 designating forward in a left-handed coordinate system
        public static readonly Vector3 ForwardLH = new Vector3(0, 0, 1);
        //A unit Vector3 designating forward in a right-handed coordinate system
        public static readonly Vector3 ForwardRH = new Vector3(0, 0, -1);

        public Vector3 RotatePoint { get; set; }
        

        public Vector3 Position { get; set; }
        public float NearPlaneDistance { get; set; }
        public Vector3 LookDirection { get; set; }
        public Vector3 UpDirection { get; set; }
        public float FarPlaneDistance { get; set; }

        public Matrix4x4 ViewMatrix { get; protected set; }
        public Matrix4x4 ProjectionMatrix { get; protected set; }

        public Vector3 Target => Position + LookDirection;

        protected GeneralCameraComponent() {

        }

        public Matrix4x4 UpdateViewMatrix() {
            ViewMatrix = Matrix4x4.CreateLookAt(Position, Target, UpDirection);
            return ViewMatrix;
        }
        public abstract Matrix4x4 UpdateProjectionMatrix(float width, float height);

        public CameraState GetState() {
            return new CameraState {
                LookDirection = LookDirection,
                UpDirection = UpDirection,
                Position = Position,
                Target = Target,

                ViewMatrix = ViewMatrix,
                ProjectionMatrix = ProjectionMatrix
            };
        }

        public abstract void ResetToDefault();
    }
}