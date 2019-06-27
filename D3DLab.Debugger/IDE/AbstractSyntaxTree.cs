using System;
using System.Collections.Generic;
using System.Linq;

namespace D3DLab.Debugger.IDE {
    abstract class AbstractSyntaxTree {
        protected Func<LexerNode, bool> BySemicolonPredicate = x => ShaderSyntaxInfo.Semicolon == x.Value;

        protected Queue<LexerNode> DelimitedBy(Queue<LexerNode> lexer, DelimeterParams _params) {
            //var next = lexer.Dequeue();
            //if (next.Token != LexerTokens.Punctuation && next.Value != _params.start) {//incorect declaration
            //    throw new SyntaxException(next);
            //}
            var next = lexer.Dequeue();
            var blocks = new Queue<LexerNode>();
            while (!_params.IsStop(next)) {
                switch (next.Token) {
                    case LexerTokens.Punctuation:
                        if (_params.IsSeparator(next)) {
                            continue;
                        }
                        if (_params.IsStop(next)) {
                            break;
                        }
                        break;
                    case LexerTokens.Comment:
                        OnIgnored(next);
                        continue;
                }
                blocks.Enqueue(next);
                if (!lexer.Any()) {
                    break;
                }
                next = lexer.Dequeue();
            }
            return blocks;
        }
        protected Queue<LexerNode> DelimitedTill(Queue<LexerNode> lexer, Func<LexerNode,bool> predicate)  {
            var res = new Queue<LexerNode>();
            while (lexer.Any()) {
                var lex = lexer.Dequeue();
                if (predicate(lex)) {
                    return res;
                }
                res.Enqueue(lex);
            }
            return res;
        }
        protected Boundary DelimitedBy(Queue<LexerNode> lexer,
            DelimeterParams _params,
            ListNode node,
            Func<Queue<LexerNode>, AbstractNode> parser) {
            var boundary = new Boundary();
            boundary.StartPointer = lexer.Peek();
            if (_params.IsStart != null && !_params.IsStart(lexer.Dequeue())) {//incorect declaration
                throw new SyntaxException(lexer.Peek());
            }
            var blocks = new Queue<LexerNode>();
            LexerNode next = null;
            var bracket = 1;
            do {
                lexer.ThrowIfEmpty(next ?? boundary.StartPointer);
                next = lexer.Dequeue();

                if (next.Token == LexerTokens.Comment) {
                    //OnIgnored(next);
                    node.Add(new CommentsNode() { Lex = next });
                    continue;
                }
                if (_params.IsStart != null && _params.IsStart(next)) {
                    bracket++;
                }

                if ((_params.IsSeparator(next) && bracket == 1) || !lexer.Any()) {
                    if (blocks.Any()) {
                        node.Add(parser(blocks));
                        blocks.ThrowIfAny();
                    }
                    continue;
                }

                if (_params.IsStop(next)) {
                    bracket--;
                    if (bracket == 0) {
                        if (blocks.Any()) {
                            node.Add(parser(blocks));
                            blocks.ThrowIfAny();
                        }
                    }
                }

                blocks.Enqueue(next);
            } while (bracket != 0 && lexer.Any());
            boundary.EndPointer = next;
            return boundary;
        }

        protected class DelimeterParams {
            /// <summary>
            /// ( , )
            /// </summary>
            public static DelimeterParams Function {
                get {
                    return new DelimeterParams {
                        IsStart = new Func<LexerNode, bool>(lex => lex.Value == "("),
                        IsStop = new Func<LexerNode, bool>(lex => lex.Value == ")"),
                        IsSeparator = new Func<LexerNode, bool>(lex => lex.Value == ",")
                    };
                }
            }
            /// <summary>
            /// [ , ]
            /// </summary>
            public static DelimeterParams Attribute {
                get {
                    return new DelimeterParams {
                        IsStart = new Func<LexerNode, bool>(lex => lex.Value == "["),
                        IsStop = new Func<LexerNode, bool>(lex => lex.Value == "]"),
                        IsSeparator = new Func<LexerNode, bool>(lex => lex.Value == ",")
                    };
                }
            }
            /// <summary>
            ///  start '{' 
            ///  separator ';' 
            ///  end '}'
            /// </summary>
            public static DelimeterParams Sequences {
                get {
                    return new DelimeterParams {
                        IsStart = new Func<LexerNode, bool>(lex => lex.Value == "{"),
                        IsStop = new Func<LexerNode, bool>(lex => lex.Value == "}"),
                        IsSeparator = new Func<LexerNode, bool>(lex => lex.Value == ";")
                    };
                }
            }
            /// <summary>
            /// stop ';'
            /// </summary>
            public static DelimeterParams VarDeclaration {
                get {
                    return new DelimeterParams {
                        IsStop = new Func<LexerNode, bool>(lex => lex.Value == ";")
                    };
                }
            }
            public Func<LexerNode, bool> IsStart = new Func<LexerNode, bool>(lex => false);
            public Func<LexerNode, bool> IsStop = new Func<LexerNode, bool>(lex => false);
            public Func<LexerNode, bool> IsSeparator = new Func<LexerNode, bool>(lex => false);
            public Func<LexerNode, bool> Is = new Func<LexerNode, bool>(lex => false);
        }

        protected abstract void OnIgnored(LexerNode lex);
    }
}
