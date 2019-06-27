using D3DLab.Std.Engine.Core.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace D3DLab.Std.Engine.Core {
    public interface IAppWindow {
        float Width { get; }
        float Height { get; }
        bool IsActive { get; }

        IntPtr Handle { get; }
        IInputManager InputManager { get; }

        event Action Resized;

        WaitHandle BeginInvoke(Action action);

        void SetTitleText(string txt);
    }
}
