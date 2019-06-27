using System.Collections.Generic;

namespace D3DLab.Std.Standard.Engine.Core {
    public interface IComponentManager {
        ID3DComponent AddComponent(ElementTag tagEntity, ID3DComponent com);
        void RemoveComponent(ElementTag tagEntity, ID3DComponent com);
        T GetComponent<T>(ElementTag tagEntity) where T : ID3DComponent;
        IEnumerable<T> GetComponents<T>(ElementTag tagEntity) where T : ID3DComponent;
        IEnumerable<ID3DComponent> GetComponents(ElementTag tagEntity);
        bool Has<T>(ElementTag tag) where T : ID3DComponent;
    }

}
