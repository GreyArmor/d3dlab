﻿using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Ext {
    public static class NumericsToDrawingConvertorsEx {
        public static System.Numerics.Vector2 ToNumericsV2(this System.Drawing.Point v2) {
            return new System.Numerics.Vector2(v2.X, v2.Y);
        }

        public static System.Drawing.Point ToDrawingPoint(this D3DLab.Std.Engine.Core.Input.WindowPoint p) {
            return new System.Drawing.Point(p.X, p.Y);
        }
       
        public static D3DLab.Std.Engine.Core.Input.WindowPoint ToWindowPoint(this System.Drawing.Point p) {
            return new D3DLab.Std.Engine.Core.Input.WindowPoint(p.X, p.Y);
        }
    }
}
