using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace D3DLab.Std.Engine.Core.Components {
    public interface ILightComponent : IGraphicComponent {
        int Index { get; }
        LightTypes Type { get; }
        float Intensity { get; }
        Vector3 Position { get; }
        Vector3 Direction { get; }
    }
}
