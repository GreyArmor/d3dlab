﻿using D3DLab.SDX.Engine.Components;
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
            {   //systems creating
                var smanager = Context.GetSystemManager();

                smanager.CreateSystem<InputSystem>();
                smanager.CreateSystem<D3DCameraSystem>();
                smanager.CreateSystem<LightsSystem>();
                smanager.CreateSystem<MovementSystem>();
                smanager.CreateSystem<MovingOnHeightMapSystem>();
                smanager.CreateSystem<AnimationSystem>();
                smanager
                    .CreateSystem<RenderSystem>()
                    .Init(device)
                    .CreateNested<SkyGradientColoringRenderTechnique>()
                    .CreateNested<SkyPlaneWithParallaxRenderTechnique>();

            }
            var em = Context.GetEntityManager();
            EngineInfoBuilder.Build(em, Window);

            /*
            var cameraTag = new ElementTag("CameraEntity");
            {   //default entities
                var em = Context.GetEntityManager();

                EngineInfoBuilder.Build(em, Window);

                em.CreateEntity(cameraTag)
                    //.AddComponent(new OrthographicCameraComponent(Window.Width, Window.Height));
                    .AddComponent(new PerspectiveCameraComponent());

                em.CreateEntity(new ElementTag("AmbientLight"))
                    .AddComponent(new D3DLightComponent {
                        Index = 0,
                        Intensity = 0.2f,
                        //Position = Vector3.Zero + Vector3.UnitZ * 1000,
                        Type = LightTypes.Ambient })
                    .AddComponent(new ColorComponent { Color = new Vector4(1,1,1,1) });

                em.CreateEntity(new ElementTag("PointLight"))
                    .AddComponent(new D3DLightComponent {
                        Index = 1,
                        Intensity = 0.6f,
                        //Position = new Vector3(2, 1, 0),
                        Position = Vector3.Zero + Vector3.UnitZ * 1000,
                        Type = LightTypes.Point
                    })
                    .AddComponent(new ColorComponent { Color = new Vector4(1, 1, 1, 1) });

                em.CreateEntity(new ElementTag("DirectionLight"))
                    .AddComponent(new D3DLightComponent {
                        Index = 2,
                        Intensity = 0.2f,
                        Direction = new Vector3(1, 4, 4).Normalize(),
                        Type = LightTypes.Directional
                    })
                    .AddComponent(new ColorComponent { Color = new Vector4(1, 1, 1, 1) });


            }
            {//entities ordering 
                Context.EntityOrder
                       .RegisterOrder<RenderSystem>(cameraTag, 0)
                       .RegisterOrder<InputSystem>(cameraTag, 0);
            }*/
        }
    }
}
