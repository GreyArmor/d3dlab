using System.Collections.Generic;
using D3DLab.Std.Standard.Engine.Core.Input;

namespace D3DLab.Std.Standard.Engine.Core {
    public class InputSnapshot {
        public List<InputEventState> Events { get; }
        public void AddEvent(InputEventState ev) {
            Events.Add(ev);
        }
        public void RemoveEvent(InputEventState ev) {
            Events.Remove(ev);
        }
        public InputSnapshot() {
            Events = new List<InputEventState>();
        }
    }    
}
