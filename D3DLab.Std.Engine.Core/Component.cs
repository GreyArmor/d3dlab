using D3DLab.Std.Engine.Core.Shaders;
using System;

namespace D3DLab.Std.Engine.Core {
    public interface IGraphicComponent : IDisposable {
        ElementTag Tag { get; }
        ElementTag EntityTag { get; set; }
        bool IsModified { get; set; }
        bool IsValid { get; }
        bool IsDisposed { get; }

        //TODO
        //bool IsAttachedToEntity{get;}
    }

    public abstract class GraphicComponent : IGraphicComponent {
        public bool IsModified { get; set; }
        public bool IsDisposed { get; protected set; }
        public virtual bool IsValid => true;
        public ElementTag Tag { get; }
        public ElementTag EntityTag { get; set; }

        protected GraphicComponent() {
            Tag = new ElementTag(Guid.NewGuid().ToString());
        }

        public virtual void Dispose() {
            IsDisposed = true;
        }

        public void PreProcessing() {

        }
        public void PostProcessing() {

        }
    }

    public interface IComponetAttachingProcess {

    }
}
