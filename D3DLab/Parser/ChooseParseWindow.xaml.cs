using System;
using System.Collections.Generic;
using System.Linq;
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

namespace D3DLab.Parser {
    /// <summary>
    /// Interaction logic for ChooseParseWindow.xaml
    /// </summary>
    public partial class ChooseParseWindow : Window {

        public ChooseParseViewModel ViewModel => (ChooseParseViewModel)DataContext;

        public ChooseParseWindow() {
            InitializeComponent();
        }
    }
}
