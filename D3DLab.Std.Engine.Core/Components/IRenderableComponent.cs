using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Components {
    public interface IRenderableComponent : IGraphicComponent {
        bool CanRender { get; set; }
    }
}
