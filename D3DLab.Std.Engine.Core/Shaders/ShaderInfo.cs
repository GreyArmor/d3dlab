﻿using System;
using System.Collections.Generic;
using System.IO;

namespace D3DLab.Std.Engine.Core.Shaders {
    public interface IShaderInfo {
        string Stage { get; }
        string EntryPoint { get; }
        string Name { get; }

        byte[] ReadCompiledBytes();
        void WriteCompiledBytes(byte[] bytes);

        string ReadText();
        byte[] ReadBytes();        
    }
}
