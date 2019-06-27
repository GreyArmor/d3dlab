using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace D3DLab.Debugger.Presentation.PropertiesEditor {
    public class Vector3ToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var v = (Vector3)value;
            return $"{v.X} {v.Y} {v.Z}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
    }
    
}
