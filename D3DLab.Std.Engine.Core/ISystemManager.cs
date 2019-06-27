using System.Collections.Generic;

namespace D3DLab.Std.Engine.Core {
    public interface ISystemManager {
        TSystem CreateSystem<TSystem>() where TSystem : class, IGraphicSystem;
        IEnumerable<IGraphicSystem> GetSystems();
        //void AddSystem(IComponentSystem system);
        void Dispose();
    }

}
