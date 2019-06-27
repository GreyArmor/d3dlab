using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace D3DLab.Debugger.Presentation.Avalonia {
    class ShaderEditControl : ICSharpCode.AvalonEdit.TextEditor {
        public static readonly DependencyProperty BindableTextProperty = DependencyProperty.Register(nameof(BindableText), 
            typeof(string), typeof(ShaderEditControl), new PropertyMetadata("", OnTextChanged, OnCoerceValue));

        public string BindableText {
            get { return (string)GetValue(BindableTextProperty); }
            set { SetValue(BindableTextProperty, value); }
        }

        public ShaderEditControl() {
            TextChanged += OnTextChanged;

            this.Options = new ICSharpCode.AvalonEdit.TextEditorOptions() {
                ConvertTabsToSpaces = true,
                EnableHyperlinks = true,
                EnableRectangularSelection = true,
                IndentationSize = 3,
                InheritWordWrapIndentation = true,
                RequireControlModifierForHyperlinkClick = true,
            };

            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("D3DLab.Debugger.Presentation.Avalonia.ShaderSyntaxDef.xshd")) {
                using (var reader = new System.Xml.XmlTextReader(resource)) {
                    this.SyntaxHighlighting = HighlightingLoader.Load(HighlightingLoader.LoadXshd(reader), HighlightingManager.Instance);
                }
            }
        }

        static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var document = (ICSharpCode.AvalonEdit.Document.TextDocument)sender.GetValue(DocumentProperty);
            var newValue = (e.NewValue as string) ?? string.Empty;
            if (document.Text == newValue) { return; }

            document.Text = newValue;
        }

        static object OnCoerceValue(DependencyObject sender, object e) {
            return e;
        }

        void OnTextChanged(object sender, System.EventArgs e) {
            if (BindableText == Text) { return; }

            BindableText = Text;
        }
    }
}
