using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Movements;
using D3DLab.Std.Engine.Core.Ext;
using D3DLab.Std.Engine.Core.Input.Commands;
using D3DLab.Std.Engine.Core.Utilities;
using g3;

namespace D3DLab.Std.Engine.Core.Systems {
    public class ManipulatableComponent : GraphicComponent {

    }
    public class CapturedToManipulateComponent : GraphicComponent {
        public Vector3 CapturePointWorld { get; set; }
    }

    public class TemporaryManipulateTransformKepperComponent : TransformComponent {
        readonly TransformComponent original;

        private Matrix4x4 temporary;
        public override Matrix4x4 MatrixWorld {
            get => temporary;
            set {
                temporary = value;
                IsModified = true;
            }
        }

        public TemporaryManipulateTransformKepperComponent(TransformComponent original) {
            this.original = original;
            temporary = original.MatrixWorld;
        }

        public TransformComponent Apply() {
            //original.MatrixWorld *= temporary;
            return original;
        }
    }


    public class MovementSystem : BaseEntitySystem, IGraphicSystem {

        protected override void Executing(SceneSnapshot snapshot) {
            var emanager = snapshot.ContextState.GetEntityManager();

            //foreach (var ev in snapshot.Snapshot.Events) {
            //    switch (ev) {
            //        case UnTargetUnderMouseCameraCommand ut:
            //            untarget = true;
            //            snapshot.Snapshot.RemoveEvent(ev);
            //            break;
            //    }
            //}

            foreach (var entity in emanager.GetEntities()) {
                foreach (var com in entity.GetComponents()) {
                    switch (com) {
                        case MovementComponent move:
                            move.Execute(new Handlers(entity, snapshot));
                            break;

                        case CapturedToManipulateComponent capture:
                            var istate = snapshot.Snapshot.CurrentInputState;
                            var isManiputating = entity.GetComponents<TemporaryManipulateTransformKepperComponent>();
                            var left = istate.ButtonsStates[Input.GeneralMouseButtons.Left];
                            
                            if (left.Condition == Input.ButtonStates.Released) {
                                entity.RemoveComponent(capture);
                                //apply transformation
                                var origin = isManiputating.Single().Apply();
                                entity.RemoveComponents<TemporaryManipulateTransformKepperComponent>();
                                entity.AddComponent(origin);
                                origin.IsModified = true;
                                continue;
                            }

                            if (!isManiputating.Any()) {//start manipulate
                                var orig = entity.GetComponent<TransformComponent>();
                                var temp = new TemporaryManipulateTransformKepperComponent(orig);
                                entity.RemoveComponent(orig);
                                entity.AddComponent(temp);
                                isManiputating = new[] { temp };
                            }

                            var transform = isManiputating.Single();
                            var movable = entity.GetComponent<ManipulatableComponent>();

                            var cstate = snapshot.Camera;

                            var begin = left.PointV2;
                            var end = istate.CurrentPosition;
                            var delta = (begin - end).Length();
                            var captutedPointW = capture.CapturePointWorld;

                            var rayW = snapshot.Viewport.UnProject(end, snapshot.Camera, snapshot.Window);
                            var vectorToMovePoint = rayW.Origin - captutedPointW;
                            
                            var endPoint = rayW.IntersecWithPlane(captutedPointW, -cstate.LookDirection);

                            var moveVector = endPoint - captutedPointW;
                           // moveVector.Normalize();

                            transform.MatrixWorld = Matrix4x4.CreateTranslation(moveVector);

                            //System.Diagnostics.Trace.WriteLine($" {captutedPointW} | {endPoint} | {moveVector}");

                            snapshot.Notifier.NotifyChange(transform);

                            break;
                    }
                }
            }
        }

        class Handlers : IMovementComponentHandler {
            readonly IEntityManager emanager;
            readonly GraphicEntity entity;
            readonly SceneSnapshot snapshot;

            public Handlers(GraphicEntity entity, SceneSnapshot snapshot) {
                this.emanager = snapshot.ContextState.GetEntityManager();
                this.entity = entity;
                this.snapshot = snapshot;
            }

            public void Execute(MoveCameraToTargetComponent component) {
                //detect that camera should be moved to new position

                //created camera movement component
                var movecom = new CameraMoveToPositionComponent { TargetPosition = component.TargetPosition };

                //just put com to camera entity
                //camera moving is job for Camera system
                emanager
                    .GetEntity(snapshot.CurrentCameraTag)
                    .AddComponent(movecom);

                entity.RemoveComponent(component);
            }

            public void Execute(FollowUpTargetComponent follower) {
                var target = emanager.GetEntity(follower.IsTarget).First();
                follower.Follow(entity, target);
            }

            public void Execute(TranslateMovementComponent translate) {

            }

        }
    }
}
