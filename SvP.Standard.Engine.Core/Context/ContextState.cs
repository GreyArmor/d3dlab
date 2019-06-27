using System;
using System.Collections.Generic;

namespace D3DLab.Std.Standard.Engine.Core {
    public interface IContextState {
        void BeginState();
        void EndState();
        IComponentManager GetComponentManager();
        IEntityManager GetEntityManager();
        ISystemManager GetSystemManager();
        void SwitchTo(int stateTo);
        EntityOrderContainer EntityOrder { get; }
    }

    sealed class Managers {
        public ISystemManager SManager { get; }
        public IComponentManager CManager { get; }
        public IEntityManager EManager { get; }
        public EntityOrderContainer EntityOrder { get; }
        internal Managers(IManagerChangeNotify notify) {
            EntityOrder = new EntityOrderContainer();
            this.SManager = new SystemManager(notify);
            var encom = new EntityComponentManager(notify, EntityOrder);
            this.CManager = encom;
            this.EManager = encom;
        }
    }

    public abstract class BaseContextState : IContextState {

        protected ISystemManager SystemManager => processor.Managers.SManager;
        protected IComponentManager ComponentManager => processor.Managers.CManager;
        protected IEntityManager EntityManager => processor.Managers.EManager;


        readonly ContextStateProcessor processor;

        public BaseContextState(ContextStateProcessor processor) {
            this.processor = processor;
        }

        public virtual void SwitchTo(int stateTo) {
            processor.SwitchTo(stateTo);
        }
        public virtual void EndState() { }
        public virtual void BeginState() { }

        public virtual IComponentManager GetComponentManager() { return ComponentManager; }
        public virtual IEntityManager GetEntityManager() { return EntityManager; }
        public virtual ISystemManager GetSystemManager() { return SystemManager; }
        public EntityOrderContainer EntityOrder { get { return processor.Managers.EntityOrder; } }
    }

    public sealed class ContextStateProcessor : IContextState {
        private sealed class EmptyContextState : IContextState {
            public EntityOrderContainer EntityOrder => throw new NotImplementedException();
            public void BeginState() { }
            public void EndState() { }
            public IComponentManager GetComponentManager() { throw new NotImplementedException(); }
            public IEntityManager GetEntityManager() { throw new NotImplementedException(); }
            public ISystemManager GetSystemManager() { throw new NotImplementedException(); }
            public void SwitchTo(int stateTo) { throw new NotImplementedException(); }
        }

        IContextState currentState;
        private readonly Dictionary<int, Func<ContextStateProcessor, IContextState>> states;

        private static Managers managers;
        private static readonly object _loker = new object();

        internal Managers Managers { get { return managers; } }

        public ContextStateProcessor(IManagerChangeNotify notify) {
            states = new Dictionary<int, Func<ContextStateProcessor, IContextState>>();
            states.Add(-1, x => new EmptyContextState());
            currentState = new EmptyContextState();

            if (managers == null) {
                lock (_loker) {
                    if (managers == null) {
                        managers = new Managers(notify);
                    }
                }
            }
        }

        public void AddState(int stateTo, Func<ContextStateProcessor, IContextState> func) {
            states.Add(stateTo, func);
        }

        public void SwitchTo(int stateTo) {
            currentState.EndState();
            currentState = states[stateTo](this);
            currentState.BeginState();
        }

        public ISystemManager GetSystemManager() {
            return currentState.GetSystemManager();
        }

        public IComponentManager GetComponentManager() {
            return currentState.GetComponentManager();
        }

        public IEntityManager GetEntityManager() {
            return currentState.GetEntityManager();
        }

        public EntityOrderContainer EntityOrder {
            get { return currentState.EntityOrder; }
        }

        public void BeginState() {
            currentState.BeginState();
        }

        public void EndState() {
            currentState.EndState();
        }
    }
}
