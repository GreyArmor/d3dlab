using D3DLab.Std.Engine.Common;
using D3DLab.Std.Engine.Core;

namespace D3DLab.Std.Engine.Components {
    public interface IRenderableComponent : IGraphicComponent {
        void Update(VeldridRenderState state);
        void Render(VeldridRenderState state);
    }

    public interface IGeometryComponent : IGraphicComponent {
        Geometry3D Geometry { get; }
    }
}
