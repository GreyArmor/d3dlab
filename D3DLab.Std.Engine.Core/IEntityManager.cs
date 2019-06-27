using System;
using System.Collections.Generic;

namespace D3DLab.Std.Engine.Core {
    public interface IEntityManager : ISynchronizationContext {
        bool HasChanges { get; }

        GraphicEntity CreateEntity(ElementTag tag);
        IEnumerable<GraphicEntity> GetEntities();
        GraphicEntity GetEntity(ElementTag tag);
        IEnumerable<GraphicEntity> GetEntity(Func<GraphicEntity, bool> predicate);
        bool IsExisted(ElementTag tag);

        void SetFilter(Func<ElementTag, bool> predicate);
        void Dispose();

        void RemoveEntity(ElementTag elementTag);

        void FrameSynchronize(int theadId);
        void PushSynchronization();
    }

}
