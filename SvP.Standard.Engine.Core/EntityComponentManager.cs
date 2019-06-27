using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DLab.Std.Standard.Engine.Core {
    public sealed class EntityComponentManager : IEntityManager, IComponentManager {

        #region IEntityManager

        readonly Dictionary<ElementTag, Entity> entities = new Dictionary<ElementTag, Entity>();
        Func<Entity, bool> predicate = x => true;
        readonly IManagerChangeNotify notify;

        public Entity CreateEntity(ElementTag tag) {
            var en = new Entity(tag, this, orderContainer);
            entities.Add(tag, en);
            notify.NotifyChange(en);
            components.Add(en.Tag, new List<ID3DComponent>());
            return en;
        }
        public IEnumerable<Entity> GetEntities() {
            return entities.Values.Where(predicate);
        }
        public Entity GetEntity(ElementTag tag) {
            return entities[tag];
        }
        public void SetFilter(Func<Entity, bool> predicate) {
            this.predicate = predicate;
        }
        #endregion

        #region IComponentManager
        readonly Dictionary<ElementTag, List<ID3DComponent>> components = new Dictionary<ElementTag, List<ID3DComponent>>();

        public ID3DComponent AddComponent(ElementTag tagEntity, ID3DComponent com) {
            com.EntityTag = tagEntity;
            components[tagEntity].Add(com);
            return com;
        }
        public void RemoveComponent(ElementTag tagEntity, ID3DComponent com) {
            components[tagEntity].Remove(com);
        }
        public T GetComponent<T>(ElementTag tagEntity) where T : ID3DComponent {
            return components[tagEntity].OfType<T>().FirstOrDefault();
        }
        public IEnumerable<T> GetComponents<T>(ElementTag tagEntity) where T : ID3DComponent {
            return components[tagEntity].OfType<T>();
        }
        public IEnumerable<ID3DComponent> GetComponents(ElementTag tagEntity) {
            return components[tagEntity].ToArray();
        }
        public bool Has<T>(ElementTag tag) where T : ID3DComponent {
            return components[tag].Any(x => x is T);
        }

        readonly EntityOrderContainer orderContainer;

        #endregion

        public EntityComponentManager(IManagerChangeNotify notify, EntityOrderContainer orderContainer) {
            this.orderContainer = orderContainer;
            this.notify = notify;
        }
    }
}
