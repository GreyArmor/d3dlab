using D3DLab.Std.Engine.Core;
using Veldrid;
using Veldrid.Utilities;
using D3DLab.Std.Engine.Core.Shaders;
using D3DLab.Std.Engine.Shaders;
using D3DLab.Std.Engine.Common;
using System.Linq;

namespace D3DLab.Std.Engine.Components {
    public abstract class ShaderComponent : GraphicComponent, IShaderEditingComponent {
        public IVeldridShaderSpecification Shader { get; }
        public DeviceBufferesUpdater Bufferes { get; }
        public ResourcesUpdater Resources { get; }

        public IRenderTechniquePass Pass { get { return Shader.passes[0]; } }

        protected ShaderComponent(IVeldridShaderSpecification shader, DeviceBufferesUpdater deviceBufferes) {
            Shader = shader;
            Bufferes = deviceBufferes;
            Resources = new ResourcesUpdater(Shader.GetResourceDescription());
        }

        public IShaderCompilator GetCompilator() {
            return new D3DShaderCompilator(Shader.passes[0].ShaderInfos);
        }
        public void ReLoad() {
            Shader.ReLoad();
        }

    }
}
