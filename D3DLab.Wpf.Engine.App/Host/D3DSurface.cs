using D3DLab.Core.Input;
using SharpDX;
using System.Windows.Forms;

namespace D3DLab.Core.Host {
    public sealed class D3DSurface : UserControl, IControlWndMessageRiser, IViewportControl {
        public D3DSurface() {
            BackColor = System.Drawing.Color.Black;
        }

        void IControlWndMessageRiser.WndProc(ref Message m) { 
            WndProc(ref m);
        }
        protected override CreateParams CreateParams {
            get {
                var prms = base.CreateParams;
                prms.Style |= 0x00010000; //WS_TABSTOP
                return prms;
            }
        }

        #region IViewportControl
        public Matrix GetViewportTransform() {
            return new Matrix(
                Width / 2f,
                0,
                0,
                0,
                0,
                -Height / 2f,
                0,
                0,
                0,
                0,
                1,
                0,
                Width / 2f,
                Height / 2f,
                0,
                1);
        }
        #endregion
    }
}
