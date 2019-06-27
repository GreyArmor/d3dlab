using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Systems {
    public interface IAnimationComponent : IGraphicComponent {
        void Animate(GraphicEntity owner, TimeSpan frameRateTime);
    }
    public class EmptyAnimationSystem : BaseEntitySystem, IGraphicSystem {
        protected override void Executing(SceneSnapshot snapshot) {
            var emanager = snapshot.ContextState.GetEntityManager();

            foreach (var entity in emanager.GetEntities()) {
                foreach(var com in entity.GetComponents<IAnimationComponent>()) {
                    com.Animate(entity, snapshot.FrameRateTime);
                }
            }
        }
    }
}
