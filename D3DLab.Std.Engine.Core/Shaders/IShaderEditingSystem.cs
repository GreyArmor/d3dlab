﻿using D3DLab.Std.Engine.Core.Shaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Shaders {
    public interface IShaderEditingSystem {
        IRenderTechniquePass[] Pass { get; }
        IShaderCompilator GetCompilator();
    }
}
