using System;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace D3DLab.Wpf.Engine.App.Host {
    public class FormsHost : WindowsFormsHost {
        public event Action<WinFormsD3DControl> HandleCreated = x => { };
        public FormsHost() {
            this.Loaded += OnLoaded;
            this.Unloaded += Unloded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            var control = new WinFormsD3DControl();
            control.HandleCreated += OnHandleCreated;
            Child = control;
        }
        
        private void OnHandleCreated(object sender, EventArgs e) {
            HandleCreated((WinFormsD3DControl)sender);
//            Viewport = new D3DViewport((System.Windows.Forms.Control)sender);
//            Viewport.Run();
        }
        

        private void Unloded(object sender, RoutedEventArgs e) {
//            Viewport.Stop();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
        }
    }
}