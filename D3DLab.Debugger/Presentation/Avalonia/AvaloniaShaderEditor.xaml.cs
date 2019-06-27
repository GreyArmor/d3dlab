using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace D3DLab.Debugger.Presentation.Avalonia {
    /// <summary>
    /// Interaction logic for AvaloniaShaderEditor.xaml
    /// </summary>
    public partial class AvaloniaShaderEditor : Window {
        public AvaloniaShaderEditor() {
            InitializeComponent();      
        }
    }
}
