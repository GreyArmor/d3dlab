using D3DLab.Std.Standard.Engine.Core;
using System;

namespace D3DLab.Std.Standard.Engine.Core {
    public class SceneSnapshot {
        public IContextState State { get; }
        public Standard.Engine.Core.InputSnapshot Snapshot { get; }
        public TimeSpan FrameRateTime { get; }
        public SceneSnapshot(IContextState state, Standard.Engine.Core.InputSnapshot isnapshot, TimeSpan time) {
            State = state;
            Snapshot = isnapshot;
            FrameRateTime = time;
        }
    }
}
