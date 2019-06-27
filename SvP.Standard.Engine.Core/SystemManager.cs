using System;
using System.Collections.Generic;

namespace D3DLab.Std.Standard.Engine.Core {
    public sealed class SystemManager : ISystemManager {
        readonly List<IComponentSystem> systems = new List<IComponentSystem>();
        public TSystem CreateSystem<TSystem>() where TSystem : class, IComponentSystem {
            var sys = Activator.CreateInstance<TSystem>();
            systems.Add(sys);
            notify.NotifyChange(sys);
            return sys;
        }
        public IEnumerable<IComponentSystem> GetSystems() {
            return systems;
        }

        readonly IManagerChangeNotify notify;

        public SystemManager(IManagerChangeNotify notify) {
            this.notify = notify;
        }
    }
}
