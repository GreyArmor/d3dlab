using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace D3DLab.Std.Engine.Core.Common {
    /// <summary>
    /// Represents a 32-bit color (4 bytes) in the form of RGBA (in byte order: R, G, B, A). 
    /// </summary>
    public struct ColorB {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public static explicit operator Vector4(ColorB value) {
            return new Vector4(value.A,value.R,value.G, value.B);
        }
    }
}
