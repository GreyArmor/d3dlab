using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Input
{
    public interface IInputPublisher {
        bool AnySubscrubers();
        void Dispose();
        void Subscrube(InputObserver s);
        void UnSubscruber(InputObserver s);
    }
}
