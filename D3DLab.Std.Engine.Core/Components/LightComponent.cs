using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace D3DLab.Std.Engine.Core.Components {
    public class LightComponent : GraphicComponent, ILightComponent {
        /// <summary>
        /// 0 - 1 range
        /// </summary>
        public float Intensity { get; set; }
        public int Index { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public LightTypes Type { get; set; }
    }
}
