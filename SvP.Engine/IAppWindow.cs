using System;
using System.Collections.Generic;
using System.Text;

namespace SvP.Engine {
    public interface IAppWindow {
        float Width { get; }
        float Height { get; }
        bool IsActive { get; }

        IntPtr Handle { get; }

        /// <summary>
        /// TODO: rremale this 
        /// need only for Sdl
        /// </summary>
        void ClearInput();
    }
}
