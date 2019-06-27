using D3DLab.Std.Engine.Systems;
using D3DLab.Std.Engine.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using D3DLab.Std.Engine.Entities;
using System.Threading.Tasks;
using System.Threading;
using D3DLab.Std.Engine.Core.Input;
using D3DLab.Std.Engine.Core.Render;

namespace D3DLab.Std.Engine {

    public class VeldridViewportState : ViewportState {
        public DeviceBuffer ProjectionBuffer;
        public DeviceBuffer ViewBuffer;
    }

    public class VeldridRenderState {
        public VeldridViewportState Viewport = new VeldridViewportState();
        public float Ticks;
        public GraphicsDevice GrDevice;
        public DisposeCollectorResourceFactory Factory;
        public IAppWindow Window;
        public CommandList Commands;
    }
  
    class GD {
        public static GraphicsDevice Create(IAppWindow window) {
            GraphicsDeviceOptions options = new GraphicsDeviceOptions(false, PixelFormat.R16_UNorm, true);
            return GraphicsDevice.CreateD3D11(options, window.Handle, (uint)window.Width, (uint)window.Height);
        }
    }

    public class Game : EngineCore {
        public readonly GraphicsDevice gd;
        public readonly DisposeCollectorResourceFactory factory;

        public Game(IAppWindow window, IContextState context): base(window,context) {
            this.gd = GD.Create(window);//for test
            this.factory = new DisposeCollectorResourceFactory(gd.ResourceFactory);
        }

        public override void Dispose() {
            base.Dispose();
            gd.WaitForIdle();
            factory.DisposeCollector.DisposeAll();
            gd.Dispose();
        }

        protected override void Initializing() {
            
        }
    }
}
