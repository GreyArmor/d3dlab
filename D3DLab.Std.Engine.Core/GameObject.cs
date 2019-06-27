using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Movements;
using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core {
    public abstract class GameObject {
        public string Description { get; }

        protected GameObject(string desc) {
            Description = desc;
        }

        public abstract void Hide(IEntityManager manager);
        public abstract void Show(IEntityManager manager);

        public virtual void ShowDebugVisualization(IEntityManager manager) {
        }
        public virtual void HideDebugVisualization(IEntityManager manager) {
        }

        public virtual void LookAtSelf(IEntityManager manager) {
        }

        protected void MoveTo(IEntityManager manager, ElementTag targetEntity) {
            var com = new MoveCameraToTargetComponent { Target = targetEntity };

            manager.GetEntity(targetEntity).AddComponent(com);
        }

        protected virtual void UppdateGeometry(IEntityManager manager, ElementTag tag, GeometryComponent geo) {
            var en = manager.GetEntity(tag);
            en.RemoveComponents<GeometryComponent>();
            en.AddComponent(geo);
        }

        public virtual void Cleanup(IEntityManager manager) {

        }
        
        public virtual IEnumerable<GraphicEntity> GetEntities(IEntityManager manager) {
            return new GraphicEntity[0];
        }
    }
}
