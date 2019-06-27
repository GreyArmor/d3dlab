using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace D3DLab.Debugger.IDE {
    enum LexerTokens {
        Punctuation,    // { } , ; .
        Number,
        String,
        Keyword,        // struct, cbuffer, float4, 
        Identifier,
        Operator,       // '+' '!=' 
        Comment,

    }

    class LexerNode {
        public readonly LexerTokens Token;
        public readonly string Value;
        public readonly TextPointer StartPointer;
        public LexerNode(LexerTokens token, string value, TextPointer start) {
            System.Diagnostics.Contracts.Contract.Ensures(start == null);
            Token = token;
            Value = value;
            StartPointer = start;
        }
        public override string ToString() {
            return $"{Token} '{Value}'";
        }
    }

    class LexicalAnalyzer {

        public readonly Queue<LexerNode> Lexers;
        readonly HashSet<string> allkeys;
        //int pos = 1;
        int line = 1;
        int index = 0;
        readonly int col = 0;

        TextPointer startPointer;
        readonly ShaderSyntaxInfo syntax;

        public LexicalAnalyzer(ShaderSyntaxInfo syntax) {
            Lexers = new Queue<LexerNode>();
            this.allkeys = syntax.AllKeys;
            this.syntax = syntax;
        }

        public void Parse(TextPointer startPointer, string text) {
            this.startPointer = startPointer;


            var builder = new StringBuilder();

            for (; index < text.Length; index++) {
                char _char = text[index];
                // pos++;

                builder.Append(_char);

                if (char.IsWhiteSpace(_char)) {
                    switch (_char) {
                        case '\n':
                            line++;
                            break;
                    }
                    // pos += 1;
                    continue;
                }
                if (IsComments(ref _char, ref text)) {
                    continue;
                }

                if (char.IsDigit(_char)) {
                    var val = FindTextBy(GetEnumerator(text, index), x => !char.IsDigit(x) && x != '.').ToString();
                    Lexers.Enqueue(new LexerNode(LexerTokens.Number, val, GetCurrentPoiter(val)));

                    //  pos += val.Length;
                    index += val.Length - 1;
                    continue;
                }
                // || _char== '\''
                if (_char == '"') {
                    var pointer = GetCurrentPoiter("");
                    //skip first char with " or '
                    var val = string.Format("\"{0}", FindTextWithLastCharBy(GetEnumerator(text, index + 1), x => x == '"'));
                    Lexers.Enqueue(new LexerNode(LexerTokens.String, val, pointer));
                    index += val.Length - 1;
                    continue;
                }

                if (IsText(ref _char)) {
                    var val = FindTextBy(GetEnumerator(text, index), x => !IsTextOrDigit(ref x) && x != '.').ToString();

                    //if this is not keyword than identifier
                    Lexers.Enqueue(new LexerNode(allkeys.Contains(val) ? LexerTokens.Keyword : LexerTokens.Identifier,
                        val, GetCurrentPoiter(val)));

                    index += val.Length - 1;
                    continue;
                }

                if (syntax.IsOperator(_char)) {
                    // ':' end of property declaration
                    Lexers.Enqueue(new LexerNode(LexerTokens.Operator, _char.ToString(), GetCurrentPoiter(_char.ToString())));
                    continue;
                }

                if (char.IsPunctuation(_char)) {//,;(){}[]
                    Lexers.Enqueue(new LexerNode(LexerTokens.Punctuation, _char.ToString(), GetCurrentPoiter(_char.ToString())));
                    //  pos += 1;
                    continue;
                }

            }
        }

        void UpdateIndexes(char[] chars) {
            //  pos += chars.Length;
            index += chars.Length - 1;
        }

        bool IsComments(ref char _char, ref string text) {
            if (_char == '/' && text[index + 1] == '*') {
                var endChars = 2;
                var val = FindTextWithLastCharBy(GetEnumerator(text, index), endChars, x => x[0] == '*' && x[1] == '/').ToString();
                Lexers.Enqueue(new LexerNode(LexerTokens.Comment, val, GetCurrentPoiter(val)));
                index += val.Length - 1;
                return true;
            }
            if (_char == '/' && text[index + 1] == '/') {
                var val = FindTextWithLastCharBy(GetEnumerator(text, index), x => x == '\n').ToString();
                line++;
                Lexers.Enqueue(new LexerNode(LexerTokens.Comment, val, GetCurrentPoiter(val)));
                index += val.Length - 1;
                return true;
            }
            return false;
        }

        TextPointer GetCurrentPoiter(string val) {
            var pointer = startPointer.GetPositionAtOffset(index + 2, LogicalDirection.Backward);

            //debug
            //var d = new TextRange(
            //    startPointer.GetPositionAtOffset(index + 2, LogicalDirection.Backward),
            //    startPointer.GetPositionAtOffset(index + 2, LogicalDirection.Forward).GetPositionAtOffset(val.Length)
            //    ).Text;

            var range = new TextRange(pointer, pointer.GetPositionAtOffset(val.Length, LogicalDirection.Forward));
            if (range.Text != val) {

            }
            //

            return pointer;
        }

        static bool IsText(ref char _char) {
            return char.IsLetter(_char) || _char == '_';
        }
        static bool IsTextOrDigit(ref char _char) {
            return char.IsLetterOrDigit(_char) || _char == '_';
        }

        static StringBuilder FindTextWithLastCharBy(IEnumerator<char> enu, Func<char, bool> predicate) {
            var sb = new StringBuilder();
            while (enu.MoveNext()) {//get all name letters 
                var currrent = enu.Current;
                if (predicate(currrent)) {
                    sb.Append(currrent);
                    break;
                }
                sb.Append(currrent);
            }
            return sb;
        }

        static StringBuilder FindTextBy(IEnumerator<char> enu, Func<char, bool> predicate) {
            var sb = new StringBuilder();
            while (enu.MoveNext()) {//get all name letters 
                var currrent = enu.Current;
                if (predicate(currrent)) {
                    break;
                }
                sb.Append(currrent);
            }
            return sb;
        }
        static StringBuilder FindTextWithLastCharBy(IEnumerator<char> enu, int count, Func<char[], bool> predicate) {
            var sb = new StringBuilder();
            var chars = new char[count];
            var index = 0;
            while (enu.MoveNext()) {//get all name letters 
                var currrent = enu.Current;

                chars[index] = currrent;
                index++;

                if (index == count) {
                    if (predicate(chars)) {
                        sb.Append(chars);
                        break;
                    }
                    index = 0;
                    sb.Append(chars);
                }
            }
            return sb;
        }
        static StringBuilder FindTextBy(IEnumerator<char> enu, int count, Func<char[], bool> predicate) {
            var sb = new StringBuilder();
            var chars = new char[count];
            var index = 0;
            while (enu.MoveNext()) {//get all name letters 
                var currrent = enu.Current;

                chars[index] = currrent;
                index++;

                if (index == count) {
                    if (predicate(chars)) {
                        break;
                    }
                    index = 0;
                    sb.Append(chars);
                }
            }
            return sb;
        }
        static IEnumerator<char> GetEnumerator(string text, int start) {
            for (int i = start; i < text.Length; i++) {
                char ch = text[i];
                yield return ch;
            }
        }
    }
}
