using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace D3DLab.Debugger.IDE {
    //https://github.com/icsharpcode/AvalonEdit/blob/master/ICSharpCode.AvalonEdit/Document/TextUtilities.cs

    static class TextHelper {
        public static string GetLastVariableName(this TextRange range) {
            var text = range.Text;
            var cleared = new StringBuilder();
            for (var i = text.Length - 1; i >= 0; i--) {
                var _char = text[i];
                if (char.IsLetterOrDigit(_char) || _char == '_') {
                    cleared.Insert(0, _char);
                } else if (cleared.Length > 0) {
                    break;
                }
            }
            return cleared.ToString();
        }

        public static string GetVariableName(this TextPointer caret) {
            var cleared = new StringBuilder();

            var docstart = caret.DocumentStart;
            var offset = caret.DocumentStart.GetOffsetToPosition(caret);

            var index = 0;
            var start = caret;
            var end = caret;
            while (true) {
                index++;
                end = docstart.GetPositionAtOffset(offset + index, LogicalDirection.Forward);
                var txt = new TextRange(start, end).Text;
                if(txt.Length == 0) {
                    continue;
                }
                var _char = txt[0];
                if (char.IsLetterOrDigit(_char) || _char == '_') {
                    cleared.Append(_char);
                } else {
                    break;
                }
                start = end;
            }
            start = caret;
            end = caret;
            index = 0;
            while (true) {
                index++;
                start = docstart.GetPositionAtOffset(offset - index, LogicalDirection.Backward);
                var txt = new TextRange(start, end).Text;
                if (txt.Length == 0) {
                    continue;
                }
                var _char = txt[0];
                if (char.IsLetterOrDigit(_char) || _char == '_') {
                    cleared.Insert(0, _char);
                } else {
                    break;
                }
                end = start;
            }

            return cleared.ToString();
        }
    }
}
