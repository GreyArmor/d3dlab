using System;

namespace D3DLab.Std.Standard.Engine.Core {
    public interface ID3DComponent : IDisposable {
        ElementTag Tag { get; }
        ElementTag EntityTag { get; set; }
    }

    public abstract class D3DComponent : ID3DComponent {
        public ElementTag Tag { get; }
        public ElementTag EntityTag { get; set; }

        protected D3DComponent() {
            Tag = new ElementTag(Guid.NewGuid().ToString());
        }

        public void Dispose() {

        }
    }
}
