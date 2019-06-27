using System;

namespace D3DLab.Debugger.IDE {
    class SyntaxException : Exception {
        public LexerNode Lexer { get; }

        public SyntaxException(LexerNode current) {
            Lexer = current;
        }
    }
}
