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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace D3DLab.Debugger.Presentation.SystemList {
    /// <summary>
    /// Interaction logic for SystemsView.xaml
    /// </summary>
    public partial class SystemsView : UserControl {
        public SystemsView() {
            InitializeComponent();

            DataContextChanged += SystemsView_DataContextChanged;
        }

        private void SystemsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            
        }
    }
}
