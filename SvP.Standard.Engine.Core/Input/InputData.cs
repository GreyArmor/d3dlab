using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace D3DLab.Std.Standard.Engine.Core.Input {
    [Flags]
    public enum GeneralMouseButtons {
        None = 0,
        Left = 1,
        Right = 2,
        Middle = 3,
        XButton1 = 4,
        XButton2 = 5
    }

    public struct WindowPoint {
        public readonly int X;
        public readonly int Y;
        public WindowPoint(int x, int y) {
            X = x;
            Y = y;
        }
    }

    #region input data

    public sealed class ButtonsState {
        // private readonly Control control;

        public ButtonsState() {
            //this.control = control;
        }

        public Vector2 PointDown { get; set; } //=> control.PointToClient(CursorPointDown).ToVector2();
        public WindowPoint CursorPointDown { get; set; }
    }
    public sealed class InputStateData {
        // private readonly Control control;
        public GeneralMouseButtons Buttons { get; set; }

        public Vector2 CurrentPosition { get; set; }//=> control.PointToClient(CursorCurrentPosition).ToVector2();

        public WindowPoint CursorCurrentPosition { get; set; }

        public int Delta { get; set; }

        // public float ControlWidth => control.Width;
        //   public float ControlHeight => control.Height;

        public IReadOnlyDictionary<GeneralMouseButtons, ButtonsState> ButtonsStates => buttonsStates;
        private readonly Dictionary<GeneralMouseButtons, ButtonsState> buttonsStates;
        public bool IsPressed(GeneralMouseButtons button) {
            return (Buttons & button) == button;
        }

        public InputStateData() {
            // this.control = control;
            buttonsStates = new Dictionary<GeneralMouseButtons, ButtonsState>();
            buttonsStates.Add(GeneralMouseButtons.Right, new ButtonsState());
            buttonsStates.Add(GeneralMouseButtons.Left, new ButtonsState());
            buttonsStates.Add(GeneralMouseButtons.Middle, new ButtonsState());
        }
    }
    public struct InputEventState {
        public int Type { get; set; }
        public InputStateData Data { get; set; }
    }
    #endregion
}
