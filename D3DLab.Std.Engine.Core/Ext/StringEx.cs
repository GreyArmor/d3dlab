using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Ext {
    public static class StringEx {
        static char[] whiteChars = new[] { ' ', '\t' };

        public static string[] SplitOnWhitespace(this string input) {
            //return oneOrMoreWhitespaces.Split(input.Trim());
            if (string.IsNullOrWhiteSpace(input)) {
                return new string[0];
            }

            return input.Split(whiteChars, System.StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
