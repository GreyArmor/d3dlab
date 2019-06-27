using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Standard.Engine.Core.Input {
    public interface IInputCommand {
        void Execute(Entity entity);
    }
}
