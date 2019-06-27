using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Movements;
using System.Linq;

namespace D3DLab.Wpf.Engine.App {
    public class SingleGameObject : GameObject {
        public ElementTag Tag { get; }

        public SingleGameObject(ElementTag tag, string desc) : base(desc) {
            Tag = tag;
        }

        public override void Hide(IEntityManager manager) {
            manager.GetEntity(Tag)
                  .GetComponent<IRenderableComponent>()
                  .CanRender = false;
        }

        public override void Show(IEntityManager manager) {
            manager.GetEntity(Tag)
                  .GetComponent<IRenderableComponent>()
                  .CanRender = true;
        }

        public override void LookAtSelf(IEntityManager manager) {
            var entity = manager.GetEntity(Tag);
            var geos = entity.GetComponents<IGeometryComponent>();
            if (geos.Any()) {
                var geo = geos.First();
                var com = new MoveCameraToTargetComponent { Target = Tag, TargetPosition = geo.Box.GetCenter() };
                entity.AddComponent(com);
            }
        }

        public override void Cleanup(IEntityManager manager) {
            base.Cleanup(manager);
            manager.RemoveEntity(Tag);
        }
    }
}
