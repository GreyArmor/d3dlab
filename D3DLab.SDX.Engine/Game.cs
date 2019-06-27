using D3DLab.SDX.Engine.Components;
using D3DLab.SDX.Engine.Rendering;
using D3DLab.Std.Engine.Core;
using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Materials;
using D3DLab.Std.Engine.Core.Ext;
using D3DLab.Std.Engine.Core.Render;
using D3DLab.Std.Engine.Core.Systems;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace D3DLab.SDX.Engine {
    internal class SDXCollision : ICollision {
        public bool Intersects(ref Std.Engine.Core.Utilities.BoundingBox box, ref Std.Engine.Core.Utilities.Ray ray) {
            float distance;
            var sbox = new BoundingBox(box.Minimum.ToSDXVector3(), box.Maximum.ToSDXVector3());
            var sray = new Ray(ray.Origin.ToSDXVector3(), ray.Direction.ToSDXVector3());
            return Collision.RayIntersectsBox(ref sray, ref sbox, out distance);
        }

        public bool Intersects(ref Std.Engine.Core.Utilities.BoundingBox box, ref Std.Engine.Core.Utilities.Ray ray, out float distance) {
            var sbox = new BoundingBox(box.Minimum.ToSDXVector3(), box.Maximum.ToSDXVector3());
            var sray = new Ray(ray.Origin.ToSDXVector3(), ray.Direction.ToSDXVector3());
            return Collision.RayIntersectsBox(ref sray, ref sbox, out distance);
        }

        public void Merge(ref Std.Engine.Core.Utilities.BoundingBox value1, ref Std.Engine.Core.Utilities.BoundingBox value2, out Std.Engine.Core.Utilities.BoundingBox result) {
            var b1 = new BoundingBox(value1.Minimum.ToSDXVector3(), value1.Maximum.ToSDXVector3());
            var b2 = new BoundingBox(value2.Minimum.ToSDXVector3(), value2.Maximum.ToSDXVector3());
            BoundingBox.Merge(ref b1, ref b2, out var res);
            result = new Std.Engine.Core.Utilities.BoundingBox(res.Minimum.ToNVector3(), res.Maximum.ToNVector3());
        }
    }
    public class D3DEngine : EngineCore {
        readonly SynchronizedGraphics device;
        public event Action<SynchronizedGraphics> Initialize;

        public D3DEngine(IAppWindow window, IContextState context, EngineNotificator notificator) :
            base(window, context, new D3DViewport(), notificator) {
            Statics.Collision = new SDXCollision();

            device = new SynchronizedGraphics(window);
        }

        protected override void OnSynchronizing() {
            device.Synchronize(System.Threading.Thread.CurrentThread.ManagedThreadId);
            base.OnSynchronizing();
        }


        public override void Dispose() {
            base.Dispose();
            device.Dispose();
        }

        protected override void Initializing() {
            Initialize?.Invoke(device);
            //{   //systems creating
            //    var smanager = Context.GetSystemManager();

            //    smanager.CreateSystem<InputSystem>();
            //    smanager.CreateSystem<D3DCameraSystem>();
            //    smanager.CreateSystem<LightsSystem>();
            //    smanager.CreateSystem<MovementSystem>();
            //    smanager.CreateSystem<MovingOnHeightMapSystem>();
            //    smanager.CreateSystem<AnimationSystem>();
            //    smanager
            //        .CreateSystem<RenderSystem>()
            //        .Init(device)
            //        .CreateNested<SkyGradientColoringRenderTechnique>()
            //        .CreateNested<SkyPlaneWithParallaxRenderTechnique>();

            //}          

            /*
            {//entities ordering 
                Context.EntityOrder
                       .RegisterOrder<RenderSystem>(cameraTag, 0)
                       .RegisterOrder<InputSystem>(cameraTag, 0);
            }*/
        }
    }
}
