using D3DLab.Std.Engine.Core;

namespace D3DLab.SDX.Engine.Components {
    public interface ID3DRenderable : IGraphicComponent {
        void Update(GraphicsDevice graphics);
        void Render(GraphicsDevice graphics);
    }
}
