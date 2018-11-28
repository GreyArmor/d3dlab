﻿using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Movements;
using D3DLab.Std.Engine.Core.Ext;
using System;
using System.Numerics;

namespace D3DLab.Std.Engine.Core.Systems {
    public class CameraSystem : BaseComponentSystem, IComponentSystem {

        protected static class Ext {
            public static bool ChangeCameraDistance(GeneralCameraComponent camera,ref float delta, Vector3 zoomAround) {
                // Handle the 'zoomAround' point
                var target = camera.Position + camera.LookDirection;
                var relativeTarget = zoomAround - target;
                var relativePosition = zoomAround - camera.Position;
                if (relativePosition.Length() < 1e-4) {
                    if (delta > 0) //If Zoom out from very close distance, increase the initial relativePosition
                    {
                        relativePosition.Normalize();
                        relativePosition /= 10;
                    } else//If Zoom in too close, stop it.
                      {
                        return false;
                    }
                }
                var f = Math.Pow(2.5, delta);
                var newRelativePosition = relativePosition * (float)f;
                var newRelativeTarget = relativeTarget * (float)f;

                var newTarget = zoomAround - newRelativeTarget;
                var newPosition = zoomAround - newRelativePosition;

                var newDistance = (newPosition - zoomAround).Length();
                var oldDistance = (camera.Position - zoomAround).Length();

                if (newDistance > camera.FarPlaneDistance && (oldDistance < camera.FarPlaneDistance || newDistance > oldDistance)) {
                    var ratio = (newDistance - camera.FarPlaneDistance) / newDistance;
                    f *= 1 - ratio;
                    newRelativePosition = relativePosition * (float)f;
                    newRelativeTarget = relativeTarget * (float)f;

                    newTarget = zoomAround - newRelativeTarget;
                    newPosition = zoomAround - newRelativePosition;
                    delta = (float)(Math.Log(f) / Math.Log(2.5));
                }

                if (newDistance < camera.NearPlaneDistance && (oldDistance > camera.NearPlaneDistance || newDistance < oldDistance)) {
                    var ratio = (camera.NearPlaneDistance - newDistance) / newDistance;
                    f *= (1 + ratio);
                    newRelativePosition = relativePosition * (float)f;
                    newRelativeTarget = relativeTarget * (float)f;

                    newTarget = zoomAround - newRelativeTarget;
                    newPosition = zoomAround - newRelativePosition;
                    delta = (float)(Math.Log(f) / Math.Log(2.5));
                }

                var newLookDirection = newTarget - newPosition;
                camera.LookDirection = newLookDirection.Normalize();
                camera.Position = newPosition;
                return true;
            }
        }

        class RotationHandler {
            readonly GeneralCameraComponent camera;
            public RotationHandler(GeneralCameraComponent camera) {
                this.camera = camera;
            }
            public void Execute(RotationComponent component) {
                var state = component.State;
                var data = component.MovementData;

                //Utilities.Helix.CameraMath.RotateTrackball(Utilities.Helix.CameraMode.Inspect,
                //    ref p11, ref p2, ref rotp, 1f, 960, 540, ccom, 1, out var newpos, out var newlook, out var newup);
                //Utilities.Helix.CameraMath.RotateTurnball(Utilities.Helix.CameraMode.Inspect,
                //    ref p11, ref p2, ref rotp, 1f, 960, 540, ccom, 1, out var newpos, out var newlook, out var newup);
                //Utilities.Helix.CameraMath.RotateTurntable(Utilities.Helix.CameraMode.Inspect,
                //    ref moveV, ref rotp, 1f, 960, 540, new  , 1, UpDirection.Value, out var newpos, out var newlook, out var newup);

                var rotateAround = camera.RotatePoint;
                var delta = data.End - data.Begin;
                var relativeTarget = rotateAround - state.Target;
                var relativePosition = rotateAround - state.Position;

                var cUp = Vector3.Normalize(state.UpDirection);
                var up = state.UpDirection;
                var dir = Vector3.Normalize(state.LookDirection);
                var right = Vector3.Cross(dir, cUp);

                float d = -0.5f;
                d *= 1;

                var xangle = d * 1 * delta.X / 180 * (float)Math.PI;
                var yangle = d * delta.Y / 180 * (float)Math.PI;

                System.Diagnostics.Trace.WriteLine($"up: {up}/{xangle}, right: {right}/{yangle}");

                var q1 = Quaternion.CreateFromAxisAngle(up, xangle);
                var q2 = Quaternion.CreateFromAxisAngle(right, yangle);
                Quaternion q = q1 * q2;

                var m = Matrix4x4.CreateFromQuaternion(q);

                var newRelativeTarget = Vector3.Transform(relativeTarget, m);
                var newRelativePosition = Vector3.Transform(relativePosition, m);

                var newTarget = rotateAround - newRelativeTarget;
                var newPosition = rotateAround - newRelativePosition;

                camera.UpDirection = Vector3.TransformNormal(cUp, m);
                camera.LookDirection = (newTarget - newPosition);
                camera.Position = newPosition;
            }
        }

        protected class PerspectiveCameraMoveHandler : IMovementComponentHandler {
            protected readonly PerspectiveCameraComponent camera;
            protected readonly SceneSnapshot snapshot;

            public PerspectiveCameraMoveHandler(PerspectiveCameraComponent camera, SceneSnapshot snapshot) {
                this.camera = camera;
                this.snapshot = snapshot;
            }

            public void Execute(RotationComponent component) {
                var rotate = new RotationHandler(camera);
                rotate.Execute(component);
            }

            public virtual void Execute(ZoomComponent component) {

                
            }
            protected bool ChangeCameraDistance(ref float delta, Vector3 zoomAround) {
                // Handle the 'zoomAround' point
                var target = camera.Position + camera.LookDirection;
                var relativeTarget = zoomAround - target;
                var relativePosition = zoomAround - camera.Position;
                if (relativePosition.Length() < 1e-4) {
                    if (delta > 0) //If Zoom out from very close distance, increase the initial relativePosition
                    {
                        relativePosition.Normalize();
                        relativePosition /= 10;
                    } else//If Zoom in too close, stop it.
                      {
                        return false;
                    }
                }
                var f = Math.Pow(2.5, delta);
                var newRelativePosition = relativePosition * (float)f;
                var newRelativeTarget = relativeTarget * (float)f;

                var newTarget = zoomAround - newRelativeTarget;
                var newPosition = zoomAround - newRelativePosition;

                var newDistance = (newPosition - zoomAround).Length();
                var oldDistance = (camera.Position - zoomAround).Length();

                if (newDistance > camera.FarPlaneDistance && (oldDistance < camera.FarPlaneDistance || newDistance > oldDistance)) {
                    var ratio = (newDistance - camera.FarPlaneDistance) / newDistance;
                    f *= 1 - ratio;
                    newRelativePosition = relativePosition * (float)f;
                    newRelativeTarget = relativeTarget * (float)f;

                    newTarget = zoomAround - newRelativeTarget;
                    newPosition = zoomAround - newRelativePosition;
                    delta = (float)(Math.Log(f) / Math.Log(2.5));
                }

                if (newDistance < camera.NearPlaneDistance && (oldDistance > camera.NearPlaneDistance || newDistance < oldDistance)) {
                    var ratio = (camera.NearPlaneDistance - newDistance) / newDistance;
                    f *= (1 + ratio);
                    newRelativePosition = relativePosition * (float)f;
                    newRelativeTarget = relativeTarget * (float)f;

                    newTarget = zoomAround - newRelativeTarget;
                    newPosition = zoomAround - newRelativePosition;
                    delta = (float)(Math.Log(f) / Math.Log(2.5));
                }

                var newLookDirection = newTarget - newPosition;
                camera.LookDirection = newLookDirection;
                camera.Position = newPosition;
                return true;
            }
            protected void ZoomByChangingFieldOfView(float delta) {
                var pcamera = camera;
                //if (!this.Controller.IsChangeFieldOfViewEnabled) {
                //    return;
                //}

                float fov = pcamera.FieldOfViewRadians;
                float d = camera.LookDirection.Length();
                float r = d * (float)Math.Tan(0.5 * fov / 180 * Math.PI);

                fov *= 1f + (delta * 0.5f);
                if (fov < camera.MinimumFieldOfView) {
                    fov = camera.MinimumFieldOfView;
                }

                if (fov > camera.MaximumFieldOfView) {
                    fov = camera.MaximumFieldOfView;
                }

                pcamera.FieldOfViewRadians = fov;
                float d2 = r / (float)Math.Tan(0.5 * fov / 180 * Math.PI);
                Vector3 newLookDirection = pcamera.LookDirection;
                newLookDirection.Normalize();
                newLookDirection *= d2;
                Vector3 target = pcamera.Position + pcamera.LookDirection;
                pcamera.Position = target - newLookDirection;
                pcamera.LookDirection = newLookDirection;
            }
        }

        protected class OrthographicCameraMoveHandler : IMovementComponentHandler {
            protected readonly OrthographicCameraComponent camera;
            protected readonly SceneSnapshot snapshot;

            public OrthographicCameraMoveHandler(OrthographicCameraComponent camera, SceneSnapshot snapshot) {
                this.camera = camera;
                this.snapshot = snapshot;
            }

            public void Execute(RotationComponent component) {
                var rotate = new RotationHandler(camera);
                rotate.Execute(component);
            }

            public virtual void Execute(ZoomComponent component) {

            }
        }

        protected virtual IMovementComponentHandler CreateHandlerOrthographicHandler(OrthographicCameraComponent com, SceneSnapshot snapshot) {
            return new OrthographicCameraMoveHandler(com, snapshot);
        }
        protected virtual IMovementComponentHandler CreateHandlerPerspectiveHandler(PerspectiveCameraComponent com, SceneSnapshot snapshot) {
            return new PerspectiveCameraMoveHandler(com, snapshot);
        }

        public void Execute(SceneSnapshot snapshot) {
            var window = snapshot.Window;
            IEntityManager emanager = snapshot.ContextState.GetEntityManager();

            try {
                foreach (var entity in emanager.GetEntities()) {
                    foreach (var com in entity.GetComponents<OrthographicCameraComponent>()) {
                        entity.GetComponents<MovementComponent>().DoFirst(movment => {
                            movment.Execute(CreateHandlerOrthographicHandler(com, snapshot));
                        });

                        com.UpdateViewMatrix();
                        com.UpdateProjectionMatrix(window.Width, window.Height);

                        snapshot.UpdateCamera(com.GetState());
                    }
                    foreach (var com in entity.GetComponents<PerspectiveCameraComponent>()) {
                        entity.GetComponents<MovementComponent>().DoFirst(movment => {
                            movment.Execute(CreateHandlerPerspectiveHandler(com, snapshot));
                        });

                        com.UpdateViewMatrix();
                        com.UpdateProjectionMatrix(window.Width, window.Height);

                        snapshot.UpdateCamera(com.GetState());
                    }

                }
            } catch (Exception ex) {
                ex.ToString();
                throw ex;
            }
        }


    }
}