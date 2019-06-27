using System.Drawing;
using System.Windows.Forms;

namespace D3DLab.Wpf.Engine.App.Host {
    public interface IControlWndMessageRiser {
        void WndProc(ref Message m);
    }

    public sealed class WinFormsD3DControl : UserControl , IControlWndMessageRiser{
       // public event Action HandleCreated = () => { };
        public WinFormsD3DControl() {
            BackColor = System.Drawing.Color.Black;
        }
        
        protected override void CreateHandle() {
            base.CreateHandle();
        }

        protected override void DestroyHandle() {

            base.DestroyHandle();
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
    }
}
