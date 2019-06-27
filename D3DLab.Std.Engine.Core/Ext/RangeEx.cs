using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Ext {
    public static class RangeEx {
        public static float ToNewRange(this float oldVal, float oldMin, float oldMax, float newMim, float newMax) {
            return (((oldVal - oldMin) * (newMax - newMim)) / (oldMax - oldMin)) + newMim;
        }
    }
}
