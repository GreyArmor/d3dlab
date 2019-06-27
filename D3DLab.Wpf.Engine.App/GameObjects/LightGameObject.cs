using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Common;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Materials;
using D3DLab.Std.Engine.Core.Components.Movements;
using D3DLab.Std.Engine.Core.Ext;
using System.Numerics;
using System.Threading;

namespace D3DLab.Wpf.Engine.App.GameObjects {
    public class LightGameObject : SingleGameObject {
        static float lightpower = 1;
        static int lights = 0;

        GameObject debugVisualObject;
        CoordinateSystemLinesGameObject coordinateSystemObject;

        public LightGameObject(ElementTag tag, string desc) : base(tag, desc) { }

        public static LightGameObject CreateDirectionLight(IEntityManager manager, Vector3 direction) {// ,
            var tag = new ElementTag("DirectionLight_" + Interlocked.Increment(ref lights));
            manager.CreateEntity(tag)
                   .AddComponents(
                       new LightComponent {
                           Index = 2,
                           Intensity = 0.2f,
                           Direction = direction,
                           Type = LightTypes.Directional
                       },
                       new ColorComponent { Color = new Vector4(1, 1, 1, 1) }
                   );

            return new LightGameObject(tag, "DirectionLight");
        }

        public static LightGameObject CreatePointLight(IEntityManager manager, Vector3 position) {// 
            var tag = new ElementTag("PointLight_" + Interlocked.Increment(ref lights));

            manager.CreateEntity(tag)
                 .AddComponents(
                     new LightComponent {
                         Index = 1,
                         Intensity = 0.4f,
                         Position = position,
                         Type = LightTypes.Point
                     },
                     new ColorComponent { Color = new Vector4(1, 1, 1, 1) }
                 );

            return new LightGameObject(tag, "PointLight");
        }

        public static LightGameObject CreateAmbientLight(IEntityManager manager) {
            var tag = new ElementTag("AmbientLight_" + Interlocked.Increment(ref lights));

            manager.CreateEntity(tag)
                   .AddComponents(
                           new LightComponent {
                               Index = 0,
                               Intensity = 0.4f,
                               //Position = Vector3.Zero + Vector3.UnitZ * 1000,
                               Type = LightTypes.Ambient
                           },
                           new ColorComponent { Color = V4Colors.White }
                       );

            return new LightGameObject(tag, "AmbientLight");
        }

        public override void ShowDebugVisualization(IEntityManager manager) {
            if (debugVisualObject.IsNotNull()) {
                debugVisualObject.Show(manager);
                coordinateSystemObject.Show(manager);
                return;
            }

            var entity = manager.GetEntity(Tag);
            var l = entity.GetComponent<LightComponent>();
            var c = entity.GetComponent<ColorComponent>();

            var center = l.Position;
            switch (l.Type) {
                case LightTypes.Point:
                    debugVisualObject = SphereGameObject.Create(manager, new SphereGameObject.Data {
                        Center = center,
                        Color = V4Colors.Red,// c.Color * l.Intensity
                    });
                    break;
            }

            coordinateSystemObject = CoordinateSystemLinesGameObject.Build(manager, center);

            base.ShowDebugVisualization(manager);
        }

        public override void HideDebugVisualization(IEntityManager manager) {
            debugVisualObject.Hide(manager);
            coordinateSystemObject.Hide(manager);

            base.HideDebugVisualization(manager);
        }

        public override void LookAtSelf(IEntityManager manager) {
            var entity = manager.GetEntity(Tag);
            var l = entity.GetComponent<LightComponent>();

            var com = new MoveCameraToTargetComponent { Target = Tag, TargetPosition = l.Position };

            manager.GetEntity(Tag).AddComponent(com);
        }
    }
}
