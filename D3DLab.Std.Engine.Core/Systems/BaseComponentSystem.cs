using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace D3DLab.Std.Engine.Core.Systems {
    public abstract class BaseEntitySystem : IComponentSystemIncrementId {
        public int ID { get; set; }
        public TimeSpan ExecutionTime { get; private set; }

        readonly Stopwatch stopwatch;
        
        protected BaseEntitySystem() {
            stopwatch = new Stopwatch();
            //var ddd = Stopwatch.IsHighResolution;
        }

        public void Execute(SceneSnapshot snapshot) {
            try {
                stopwatch.Restart();
                Executing(snapshot);
            } finally {
                stopwatch.Stop();
                ExecutionTime = stopwatch.Elapsed;
            }
        }

        protected abstract void Executing(SceneSnapshot snapshot);

        }

    public abstract class ContainerSystem<TNestedSystem> : BaseEntitySystem {
        readonly SynchronizationContext<ContainerSystem<TNestedSystem>, TNestedSystem> synchronization;
        protected readonly List<TNestedSystem> nested;

        public ContainerSystem() {
            synchronization = new SynchronizationContext<ContainerSystem<TNestedSystem>, TNestedSystem>(this);
            nested = new List<TNestedSystem>();
        }

        public ContainerSystem<TNestedSystem> CreateNested<T>() where T : TNestedSystem {
            synchronization.Add((owner, tech) => {
                owner.nested.Add(tech);
            },Activator.CreateInstance<T>());
            return this;
        }

        protected void Synchronize() {
            synchronization.Synchronize(-1);
        }
    }
}
