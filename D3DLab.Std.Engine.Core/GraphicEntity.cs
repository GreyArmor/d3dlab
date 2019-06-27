using System;
using System.Collections.Generic;

namespace D3DLab.Std.Engine.Core {

    public sealed class GraphicEntity  {
        public ElementTag Tag { get; }
        readonly IComponentManager manager;
        readonly IEntityManager emanager;
        readonly EntityOrderContainer order;

        public GraphicEntity(ElementTag tag, IComponentManager manager, IEntityManager emanager,EntityOrderContainer order) {
            this.order = order;
            this.manager = manager;
            this.emanager = emanager;
            Tag =tag;
           
        }

        public T GetComponent<T>() where T : IGraphicComponent {
            return manager.GetComponent<T>(Tag);
        }
        public IEnumerable<T> GetComponents<T>() where T : IGraphicComponent {
            return manager.GetComponents<T>(Tag);
        }
        public IEnumerable<IGraphicComponent> GetComponents(params Type[] types) {
            return manager.GetComponents(Tag, types);
        }

        public T GetOrCreateComponent<T>(T newone) where T : IGraphicComponent {
            return manager.GetOrCreateComponent<T>(Tag, newone);
        }

        public GraphicEntity AddComponent<T>(T component) where T : IGraphicComponent {
            manager.AddComponent(Tag, component);
            return this;
        }
        public GraphicEntity AddComponents(params IGraphicComponent[] components){
            manager.AddComponents(Tag, components);
            return this;
        }
        public GraphicEntity AddComponents(IEnumerable<IGraphicComponent> components) {
            manager.AddComponents(Tag, components);
            return this;
        }
        public void RemoveComponent(IGraphicComponent component) {
            manager.RemoveComponent(Tag, component);            
        }

        public void Remove() {
            emanager.RemoveEntity(Tag);
        }

        public void RemoveComponents<T>() where T : IGraphicComponent {
            manager.RemoveComponents<T>(Tag);
        }
        public void RemoveComponentsOfType<TCom>() where TCom : IGraphicComponent {
            foreach (var component in manager.GetComponents<TCom>(Tag)) {
                manager.RemoveComponent(Tag, component);
            }
        }

        public bool Has(params Type[] types) {
            return manager.Has(Tag, types);
        }
        public bool Has<T>() where T : IGraphicComponent {
            return manager.Has<T>(Tag);
        }
        public IEnumerable<IGraphicComponent> GetComponents() {
            return manager.GetComponents(Tag);
        }

        public int GetOrderIndex<TSys>()
            where TSys : IGraphicSystem  {
            return order.Get<TSys>(Tag);
        }

        public bool IsDestroyed => !emanager.IsExisted(Tag);
    }
    public class OrderSystemContainer : Dictionary<Type, int> {

    }
    public class EntityOrderContainer {
        readonly Dictionary<ElementTag, OrderSystemContainer> componentOrderIndex;
        readonly Dictionary<Type, int> systemsOrder;

        public EntityOrderContainer() {
            componentOrderIndex = new Dictionary<ElementTag, OrderSystemContainer>();
            systemsOrder = new Dictionary<Type, int>();
        }
        public EntityOrderContainer RegisterOrder<TSys>(ElementTag tag,int index) 
            where TSys : IGraphicSystem{
            OrderSystemContainer ordering;
            if (!componentOrderIndex.TryGetValue(tag, out ordering)) {
                ordering = new OrderSystemContainer();
                componentOrderIndex.Add(tag, ordering);
            }
            var t = typeof(TSys);

            ordering.Add(t, index);
            IncrementSystemOrderIndex(t);

            return this;
        }

        public EntityOrderContainer RegisterOrder<TSys>(ElementTag tag)
           where TSys : IGraphicSystem {
            OrderSystemContainer ordering;
            if (!componentOrderIndex.TryGetValue(tag, out ordering)) {
                ordering = new OrderSystemContainer();
                componentOrderIndex.Add(tag, ordering);
            }
            var t = typeof(TSys);

            ordering.Add(t, IncrementSystemOrderIndex(t));

            return this;
        }

        public int Get<TSys>(ElementTag tag)
            where TSys : IGraphicSystem {
            if (!componentOrderIndex.ContainsKey(tag)) {
                return int.MaxValue;
            }
            return componentOrderIndex[tag][typeof(TSys)];
        }

        int IncrementSystemOrderIndex(Type t) {
            if (!systemsOrder.ContainsKey(t)) {
                systemsOrder.Add(t, 0);
            } else {
                systemsOrder[t] = systemsOrder[t] + 1;
            }
            return systemsOrder[t];
        }
    }
}
