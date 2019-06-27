using D3DLab.UI.Utils;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace D3DLab.UI.Controls {
    [DefaultProperty("Child")]
    [Localizability(LocalizationCategory.None)]
    [ContentProperty("Child")]
    public class OverlayPanel : FrameworkElement, IAddChild {
        const uint NO_REPOSITION_FLAGS = Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE | Win32.SWP_NOZORDER | Win32.SWP_NOOWNERZORDER | Win32.SWP_NOREPOSITION;
        const uint SET_ONLY_LOCATION = Win32.SWP_NOACTIVATE | Win32.SWP_NOZORDER | Win32.SWP_NOOWNERZORDER;
        const uint SET_ONLY_ZORDER = Win32.SWP_NOMOVE | Win32.SWP_NOSIZE | Win32.SWP_NOACTIVATE;

        OverlayVisualRoot visualRoot;
        HwndSource hwnd;
        bool isShown;
        Rect parentBounds;
        Rect bounds;

        HwndSource HwndSource {
            get { return (HwndSource)PresentationSource.FromVisual(this); }
        }

        IntPtr Handle {
            get { return HwndSource.Handle; }
        }

        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(nameof(Child), typeof(UIElement), typeof(OverlayPanel));

        public UIElement Child {
            get { return (UIElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public OverlayPanel() {
            visualRoot = new OverlayVisualRoot(this);
            AddLogicalChild(visualRoot);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            IsVisibleChanged += OnIsVisibleChanged;
            LayoutUpdated += OnLayoutUpdated;
        }

        #region Event handlers
        void OnLoaded(object sender, RoutedEventArgs e) {
            visualRoot.Content = Child;
            CreateHwnd();
            Attach();
            //PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
            //if (HwndSource != null) { OnSourceChanged(this, new SourceChangedEventArgs(null, HwndSource)); }
        }

        void OnUnloaded(object sender, RoutedEventArgs e) {
            visualRoot.Content = null;
            DestroyHwnd();
            Detach();
            //PresentationSource.RemoveSourceChangedHandler(this, OnSourceChanged);
        }

        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            InvalidateAppearance();
        }

        void OnLayoutUpdated(object sender, EventArgs e) {
            var source = PresentationSource.FromVisual(this);
            var compositionTarget = source?.CompositionTarget;
            if (compositionTarget == null || compositionTarget.RootVisual == null) { return; }

            UpdateBoundingBox(CalculateAssignedRC(source));
        }
        #endregion

        #region Window
        void OnParentLocationChanged(object sender, EventArgs e) {
            UpdatePosition();
        }

        void OnParentSizeChanged(object sender, SizeChangedEventArgs e) {
            UpdatePosition();
        }

        void CreateHwnd() {
            if (hwnd != null) { return; }

            int classStyle = 0;
            int style = (int)Win32.WS_CLIPCHILDREN;
            int styleEx = Win32.WS_EX_NOACTIVATE;

            HwndSourceParameters parameters = new HwndSourceParameters() {
                UsesPerPixelOpacity = true,
                WindowClassStyle = classStyle,
                WindowStyle = style,
                ExtendedWindowStyle = styleEx,
                PositionX = (int)(parentBounds.X + bounds.X),
                PositionY = (int)(parentBounds.Y + bounds.Y),
                Width = (int)(bounds.Width),
                Height = (int)(bounds.Height)
            };

            hwnd = new HwndSource(parameters);
            hwnd.RootVisual = visualRoot;
            hwnd.AddHook(WndProc);

            isShown = false;
        }

        void DestroyHwnd() {
            if (hwnd == null) { return; }

            hwnd.RemoveHook(WndProc);
            hwnd.Dispose();
           // hwnd = null;

            isShown = false;
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == Win32.WM_MOUSEACTIVATE) {
                handled = true;
                return new IntPtr(Win32.MA_NOACTIVATE);
            } else if (msg == Win32.WM_ACTIVATE) {
                SetZOrder();
            } else if (msg == Win32.WM_GETMINMAXINFO) {
                unsafe {
                    MINMAXINFO* minMaxInfo = (MINMAXINFO*)lParam;
                    minMaxInfo->ptMinTrackSize = new POINT() { X = 0, Y = 0 };
                }

                // A safe inefficient version for the unsafe block above 

                //var minMaxInfo = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof (MINMAXINFO));
                //minMaxInfo.ptMinTrackSize = new POINT();
                //Marshal.StructureToPtr(minMaxInfo, lParam, true);
            } else if (msg == Win32.WM_WINDOWPOSCHANGED) {
                UpdatePosition();
            }

            return IntPtr.Zero;
        }

        void ConnectToOwner() {
            Win32.SetWindowLong(hwnd.Handle, Win32.GWL_HWNDPARENT, Handle);
        }

        void DisconnectFromOwner() {
            Win32.SetWindowLong(hwnd.Handle, Win32.GWL_HWNDPARENT, IntPtr.Zero);
        }
        #endregion

        #region Source
        void OnSourceChanged(object sender, SourceChangedEventArgs args) {
            if (args.OldSource is HwndSource oldSource) {
                OnSourceDisconnected(oldSource);
            }

            if (args.NewSource is HwndSource newSource) {
                OnSourceConnected(newSource);
            }
        }

        void OnSourceConnected(HwndSource connectedSource) {
            Detach();
            Attach(); // should pass connectedSource as arg here

            //var window = connectedSource.RootVisual as Window;
            //if (window != null) {
            //    window.SizeChanged += OnParentSizeChanged;
            //    window.LocationChanged += OnParentLocationChanged;
            //} else {
            //    connectedSource.AddHook(WndProc);
            //}

            //ConnectToOwner();
            //SetZOrder();
            //InvalidateAppearance();
            //UpdateOwnerPosition(GetRectFromRoot((UIElement)HwndSource.RootVisual));
        }

        void OnSourceDisconnected(HwndSource disconnectedSource) {
            Detach();
            //DisconnectFromOwner();
        }
        #endregion

        void Attach() {
            var window = HwndSource.RootVisual as Window;
            if (window != null) {
                window.SizeChanged += OnParentSizeChanged;
                window.LocationChanged += OnParentLocationChanged;
            } else {
                HwndSource.AddHook(WndProc);
            }

            ConnectToOwner();
            SetZOrder();
            InvalidateAppearance();
            UpdateOwnerPosition(GetRectFromRoot((UIElement)HwndSource.RootVisual));
        }

        void Detach() {
            DisconnectFromOwner();
        }

        void SetZOrder() {
            var current = Handle;

            var prev = Win32.GetWindow(current, Win32.GW_HWNDPREV);

            //TODO: potential bug here
            if (prev == Handle) {
                current = prev;
                prev = Win32.GetWindow(current, Win32.GW_HWNDPREV);
            }

            if (prev == IntPtr.Zero) {
                Win32.SetWindowPos(current, hwnd.Handle, 0, 0, 0, 0, SET_ONLY_ZORDER);
            } else {
                Win32.SetWindowPos(hwnd.Handle, prev, 0, 0, 0, 0, SET_ONLY_ZORDER);
            }
        }

        void InvalidateAppearance() {
            if (hwnd == null) { return; }

            if (IsVisible) {
                if (!isShown) {
                    isShown = true;
                    Win32.SetWindowPos(hwnd.Handle, IntPtr.Zero, 0, 0, 0, 0, NO_REPOSITION_FLAGS | Win32.SWP_SHOWWINDOW);
                }
            } else {
                if (isShown) {
                    isShown = false;
                    Win32.SetWindowPos(hwnd.Handle, IntPtr.Zero, 0, 0, 0, 0, NO_REPOSITION_FLAGS | Win32.SWP_HIDEWINDOW);
                }
            }
        }

        void UpdateBoundingBox(Rect newBounds) {
            if (bounds.Equals(newBounds)) { return; }

            bounds = newBounds;
            UpdatePosition();
        }

        Rect GetRectFromRoot(UIElement root) {
            return new Rect(root.PointToScreen(new Point()), root.PointToScreen(new Point(root.RenderSize.Width, root.RenderSize.Height)));
        }

        Rect CalculateAssignedRC(PresentationSource source) {
            var rectElement = new Rect(RenderSize);
            var rectRoot = RectUtil.ElementToRoot(rectElement, this, source);

            return RectUtil.RootToClient(rectRoot, source);
        }

        void UpdatePosition() {
            if (hwnd == null || !isShown) { return; }
            UpdateOwnerPosition(GetRectFromRoot((UIElement)HwndSource.RootVisual));

            Win32.SetWindowPos(hwnd.Handle, IntPtr.Zero,
                (int)(parentBounds.X + bounds.X),
                (int)(parentBounds.Y + bounds.Y),
                (int)(Math.Min(bounds.Width, parentBounds.Width - bounds.X)),
                (int)(Math.Min(bounds.Height, parentBounds.Height - bounds.Y)),
                SET_ONLY_LOCATION | Win32.SWP_ASYNCWINDOWPOS);
        }

        void UpdateOwnerPosition(Rect newBounds) {
            if (parentBounds.Equals(newBounds)) { return; }

            parentBounds = newBounds;
            UpdatePosition();
        }

        protected override IEnumerator LogicalChildren {
            get { return new SingleChildEnumerator(visualRoot); }
        }

        #region IAddChild
        void IAddChild.AddChild(Object value) {
            Child = value as UIElement ?? throw new ArgumentException("Overlay accepts only UIElement as a child");
        }

        void IAddChild.AddText(string text) {
            var textBlock = new TextBlock();
            textBlock.Text = text;

            Child = textBlock;
        }
        #endregion
    }

    class OverlayVisualRoot : ContentControl {
        public DependencyObject UIParentCore { get; private set; }

        public OverlayVisualRoot(FrameworkElement element) {
            UIParentCore = element;
        }

        protected override DependencyObject GetUIParentCore() {
            return UIParentCore ?? base.GetUIParentCore();
        }
    }

    #region SingleChildEnumerator
    class SingleChildEnumerator : IEnumerator {
        readonly object child;
        State state;

        public SingleChildEnumerator(object child) {
            this.child = child;
        }

        public object Current {
            get {
                if (state == State.Current) { return child; }

                throw new InvalidOperationException();
            }
        }

        public void Reset() {
            state = State.Reset;
        }

        public bool MoveNext() {
            switch (state) {
                case State.Reset:
                    state = State.Current;
                    return true;
                case State.Current:
                    state = State.Finished;
                    return false;
                case State.Finished:
                    return false;
                default:
                    return false;
            }
        }

        enum State { Reset, Current, Finished }
    }
    #endregion

    #region RectUtil
    static class RectUtil {
        internal static Rect ElementToRoot(Rect rectElement, Visual element, PresentationSource presentationSource) {
            GeneralTransform transformElementToRoot = element.TransformToAncestor(presentationSource.RootVisual);
            Rect rectRoot = transformElementToRoot.TransformBounds(rectElement);

            return rectRoot;
        }

        internal static Rect RootToClient(Rect rectRoot, PresentationSource presentationSource) {
            CompositionTarget target = presentationSource.CompositionTarget;
            Matrix matrixRootTransform = GetVisualTransform(target.RootVisual);
            Rect rectRootUntransformed = Rect.Transform(rectRoot, matrixRootTransform);
            Matrix matrixDPI = target.TransformToDevice;
            Rect rectClient = Rect.Transform(rectRootUntransformed, matrixDPI);

            return rectClient;
        }

        internal static Matrix GetVisualTransform(Visual v) {
            if (v != null) {
                Matrix m = Matrix.Identity;

                Transform transform = VisualTreeHelper.GetTransform(v);
                if (transform != null) {
                    Matrix cm = transform.Value;
                    m = Matrix.Multiply(m, cm);
                }

                Vector offset = VisualTreeHelper.GetOffset(v);
                m.Translate(offset.X, offset.Y);

                return m;
            }

            return Matrix.Identity;
        }
    }
    #endregion
}