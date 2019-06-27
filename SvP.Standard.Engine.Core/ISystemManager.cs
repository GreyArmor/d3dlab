using System.Collections.Generic;

namespace D3DLab.Std.Standard.Engine.Core {
    public interface ISystemManager {
        TSystem CreateSystem<TSystem>() where TSystem : class, IComponentSystem;
        IEnumerable<IComponentSystem> GetSystems();
        //void AddSystem(IComponentSystem system);
    }

}
