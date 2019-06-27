using SvP.Standard.Engine.Core;
using System.Collections.Generic;
using Veldrid;
using Veldrid.Utilities;
using System.Linq;
using SvP.Engine.Input;

namespace SvP.Engine.Systems {
    public class VeldridRenderSystem : IComponentSystem {
        DisposeCollectorResourceFactory factory;
        GraphicsDevice gd;
        IAppWindow window;

        public void Init(GraphicsDevice gd, DisposeCollectorResourceFactory factory, IAppWindow window) {
            this.gd = gd;
            this.factory = factory;
            this.window = window;
        }

        public void Execute(SceneSnapshot snapshot) {
            IEntityManager emanager = snapshot.State.GetEntityManager();
            Standard.Engine.Core.InputSnapshot input = snapshot.Snapshot;

            using (var _cl = factory.CreateCommandList()) {
                _cl.Begin();

                var state = new RenderState() {
                    factory = factory,
                    gd = gd,
                    window = window,
                    Commands = _cl,
                    Ticks = (float)snapshot.FrameRateTime.TotalMilliseconds
                };

                var renderables = new List<IRenderableComponent>();

                _cl.SetFramebuffer(gd.SwapchainFramebuffer);
                _cl.SetFullViewports();
                _cl.ClearColorTarget(0, RgbaFloat.Black);
                _cl.ClearDepthStencil(1f);

                foreach (var entity in emanager.GetEntities().OrderBy(x=>x.GetOrderIndex<VeldridRenderSystem>())) {
                    entity.GetComponents<IRenderableComponent>()
                        .DoFirst(x => {
                            x.Update(state);
                            renderables.Add(x);
                        });
                }

                foreach (var renderable in renderables) {
                    renderable.Render(state);
                }

                _cl.End();
                gd.SubmitCommands(_cl);
                gd.SwapBuffers();
            }
        }
    }
}
