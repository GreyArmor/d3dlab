using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Components;
using System.Threading;

namespace D3DLab.Wpf.Engine.App.GameObjects {
    public class CameraGameObject : SingleGameObject {
        static int cameras = 0;

        public CameraGameObject(ElementTag tag, string descr) : base(tag, descr) { }

        public static CameraGameObject Create(IContextState context) {
            IEntityManager manager = context.GetEntityManager();
            var cameraTag = new ElementTag("CameraEntity_" + Interlocked.Increment(ref cameras));

            var obj = new CameraGameObject(cameraTag, "PerspectiveCamera");

            manager.CreateEntity(cameraTag)
                   //.AddComponent(new OrthographicCameraComponent(Window.Width, Window.Height));
                   .AddComponent(new PerspectiveCameraComponent());

            {//entities ordering 
                context.EntityOrder
                       .RegisterOrder<SDX.Engine.Rendering.RenderSystem>(cameraTag, 0)
                       .RegisterOrder<Std.Engine.Core.Systems.InputSystem>(cameraTag, 0);
            }

            return obj;
        }

    }
}
