using D3DLab.Debugger.IDE;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace D3DLab.Debugger.Windows {
    public class TextPoiterChanged {
        public TextPointer StartPointer;
        public TextPointer EndPointer;

        public TextRange GetRange() {
            return new TextRange(StartPointer, EndPointer);
        }
    }
    public class TextBoxChangedEventArgs {
        public ShaderTextBox TextBox { get; set; }
        public TextChangedEventArgs Args { get; set; }
    }
    public class TextPoiterSelectionChanged {
        public readonly TextPointer CaretPointer;

        public TextPoiterSelectionChanged(TextPointer caretPointer) {
            CaretPointer = caretPointer;
        }

        public TextRange GetRange() {
            var range = new TextRange(CaretPointer.GetNextContextPosition(LogicalDirection.Backward), CaretPointer.GetNextContextPosition(LogicalDirection.Forward));
            range.Select(CaretPointer.GetNextContextPosition(LogicalDirection.Backward), CaretPointer.GetNextContextPosition(LogicalDirection.Forward));

            return range;
        }
    }

    public class ShaderTextBox : RichTextBox {
        public static readonly DependencyProperty ShaderDocumentProperty;
        public static readonly DependencyProperty WordSelectedCommandProperty;
        public static readonly DependencyProperty IntellisenseInvokedCommandProperty;
        public static readonly DependencyProperty DocumentChangedCommandProperty;

        static ShaderTextBox() {
            // DefaultStyleKeyProperty.OverrideMetadata(typeof(ShaderTextBox), new FrameworkPropertyMetadata(typeof(ShaderTextBox)));

            ShaderDocumentProperty = DependencyProperty.Register(
                   nameof(ShaderDocument),
                   typeof(FlowDocument),
                   typeof(ShaderTextBox),
                   new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDocumentChanged)));

            WordSelectedCommandProperty = DependencyProperty.Register(
                    nameof(WordSelectedCommand),
                    typeof(ICommand),
                    typeof(RichTextBox));
            IntellisenseInvokedCommandProperty = DependencyProperty.Register(
                    nameof(IntellisenseInvokedCommand),
                    typeof(ICommand),
                    typeof(RichTextBox));
            DocumentChangedCommandProperty = DependencyProperty.Register(
                    nameof(DocumentChangedCommand),
                    typeof(ICommand),
                    typeof(RichTextBox));
        }



        public ShaderTextBox() {

            this.SelectionChanged += ShaderTextBox_SelectionChanged;
            this.TextChanged += ShaderTextBox_TextChanged;
            //this.KeyDown += ShaderTextBox_KeyDown;
            this.KeyUp += ShaderTextBox_KeyUp;
        }

        private void ShaderTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (IntellisenseInvokedCommand == null) { return; }
            var startPoiter = this.CaretPosition;
            var endPoiter = startPoiter.GetPositionAtOffset(1);
            
            var i = (int)e.Key;
            var t = IntellisenseTypes.Dot;
            string variableName = null;

            if(Keyboard.Modifiers != ModifierKeys.None) {
                return;
            }

            if (e.Key == Key.OemPeriod) {
                t = IntellisenseTypes.Dot;
                var docstart = startPoiter.DocumentStart;
                var offset = startPoiter.DocumentStart.GetOffsetToPosition(startPoiter);
                variableName = docstart.GetPositionAtOffset(offset - 1).GetVariableName();
            } else if(44 <= i && 69 >= i) {
                t = IntellisenseTypes.AutoComplete;
                variableName = startPoiter.GetVariableName();
                endPoiter = startPoiter;
            }

            if (string.IsNullOrWhiteSpace(variableName)) { return; }

            var pr = IntellisensePopup.Build(this, endPoiter, new TextPoiterChanged { EndPointer = startPoiter, StartPointer = endPoiter });
            pr.Type = t;
            pr.VariableName = variableName;
            pr.ShaderTextBox = this;

            IntellisenseInvokedCommand.RiseCommand(pr);
            e.Handled = true;
        }

        private void ShaderTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (IntellisenseInvokedCommand == null) { return; }
            var startPoiter = this.CaretPosition;
            var name = startPoiter.GetVariableName();
            var endPoiter = startPoiter.GetPositionAtOffset(1);
            var pr = IntellisensePopup.Build(this, endPoiter, new TextPoiterChanged { EndPointer = startPoiter, StartPointer = endPoiter });

            pr.VariableName = name;

            if (e.Key == Key.OemPeriod) {
                pr.Type = IntellisenseTypes.Dot;
                IntellisenseInvokedCommand.RiseCommand(pr);
                e.Handled = true;
            }
        }

        private void ShaderTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            DocumentChangedCommand.RiseCommand(new TextBoxChangedEventArgs { Args = e, TextBox = this });
        }

        private void ShaderTextBox_SelectionChanged(object sender, RoutedEventArgs e) {
            var pointer = this.CaretPosition;
            WordSelectedCommand.RiseCommand(new TextPoiterSelectionChanged(pointer));
        }


        public ICommand IntellisenseInvokedCommand {
            get { return (ICommand)GetValue(IntellisenseInvokedCommandProperty); }
            set { SetValue(IntellisenseInvokedCommandProperty, value); }
        }

        public ICommand WordSelectedCommand {
            get { return (ICommand)GetValue(WordSelectedCommandProperty); }
            set { SetValue(WordSelectedCommandProperty, value); }
        }
        public ICommand DocumentChangedCommand {
            get { return (ICommand)GetValue(DocumentChangedCommandProperty); }
            set { SetValue(DocumentChangedCommandProperty, value); }
        }

        public FlowDocument ShaderDocument {
            get { return (FlowDocument)GetValue(ShaderDocumentProperty); }
            set { SetValue(ShaderDocumentProperty, value); }
        }

        private static void OnDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var box = (ShaderTextBox)d;
            box.Document = (FlowDocument)e.NewValue;
        }
    }
}
