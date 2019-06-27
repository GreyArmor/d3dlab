using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace D3DLab.Debugger.IDE {
    
    class SyntaxHighlighter : IASTNodeVisitor  {
        static Brush FromString(string color) {
            var converter = new System.Windows.Media.BrushConverter();
            return (Brush)converter.ConvertFromString(color);
        }

        static readonly Brush KeywordBrush = FromString("#569CD6");
        static readonly Brush NewKeywordsNameBrush = FromString("#4EC9B0");
        static readonly Brush SemanticNameBrush = FromString("#C8C8C8");
        static readonly Brush NumereticBrush = FromString("#B5CEA8");
        static readonly Brush StringBrush = FromString("#D69D85");
        static readonly Brush CommentsBrush = FromString("#57A64A");
        static readonly Brush SelectionBrush = FromString("#57A64A");

        private ShaderSyntaxInfo syntax;

        public SyntaxHighlighter(ShaderSyntaxInfo syntax) {
            this.syntax = syntax;
        }

        public void HighlightException(SyntaxException se) {
            ApplyForegroundValue(se.Lexer, Brushes.Red);
        }
        public void HighlightSelection(LexerNode lexer) {
            ApplyFontWeight(lexer, FontWeights.Bold);
            //ApplyBackgroundValue(lexer, Brushes.LightBlue);
        }
        public void UnHighlightSelection(LexerNode lexer) {
            ApplyFontWeight(lexer, FontWeights.Normal); 
            //ApplyBackgroundValue(lexer, Brushes.Transparent);
        }

        public void Visit(CommentsNode node) {
            ApplyForegroundValue(node.Lex, CommentsBrush);
        }
        public void Visit(IncludeDeclaration incdec) {
            ApplyForegroundValue(incdec.Name, SemanticNameBrush);
            incdec.Path.Handle(this);
        }
        public void Visit(FunctionDeclarationNode funcdec) {
            funcdec.VarDeclaration.Handle(this);
            if (funcdec.SemanticNameLex != null) {
                ApplyForegroundValue(funcdec.SemanticNameLex, SemanticNameBrush);
            }
            funcdec.Children.ForEach(x => x.Handle(this));
            funcdec.Vars.ForEach(x => x.Handle(this));
        }
        public void Visit(DefinitionObjectNode don) {
            don.Children.ForEach(x => x.Handle(this));
        }
        public void Visit(VariableDefinitionWitSemanticName svd) {
            svd.VarDeclaration.Handle(this);
            ApplyForegroundValue(svd.SemanticNameLex, SemanticNameBrush);
        }
        public void Visit(CBufferNode cbuff) {
            cbuff.VarDeclaration.Handle(this);
            cbuff.Register.Handle(this);
            cbuff.Children.ForEach(x => x.Handle(this));
        }
        public void Visit(StructNode str) {
            str.VarDeclaration.Handle(this);
            str.Children.ForEach(x => x.Handle(this));
        }
        public void Visit(NameNode nameNode) {

        }
        public void Visit(ArrayNameNode ar) {
            ApplyForegroundValue(ar.IndexerLex, NumereticBrush);
        }
        public void Visit(StringNode stn) {
            ApplyForegroundValue(stn.NameLex, StringBrush);
        }
        public void Visit(NumbertNode num) {
            ApplyForegroundValue(num.NameLex, NumereticBrush);
        }
        public void Visit(SystemName num) {
            ApplyForegroundValue(num.NameLex, KeywordBrush);
        }
        public void Visit(VarDeclaration decn) {
            if (decn.TypeNode != null) {
                decn.TypeNode.Handle(this);
            }
            decn.DeclarationName.Handle(this);
        }
        public void Visit(DatatypeNode dt) {
            if (dt.ModificationLex != null) {
                ApplyForegroundValue(dt.ModificationLex, KeywordBrush);
            }
            ApplyForegroundValue(dt.TypeLex,
                syntax.AllKeys.Is(dt.TypeLex.Value) ?
                    KeywordBrush : NewKeywordsNameBrush);
        }
        public void Visit(GenericTypeNode gtn) {
            if (gtn.ModificationLex != null) {
                ApplyForegroundValue(gtn.ModificationLex, KeywordBrush);
            }
            ApplyForegroundValue(gtn.TypeLex, KeywordBrush);
            ApplyForegroundValue(gtn.TTypeLex,
                syntax.AllKeys.Is(gtn.TTypeLex.Value) ?
                    KeywordBrush : NewKeywordsNameBrush);
        }
        public void Visit(PrimitiveType dt) {
            HighlightType(dt);
        }
        public void Visit(ArrayType dt) {
            HighlightType(dt);
        }
        public void Visit(StringTypeNode dt) {
            HighlightType(dt);
        }
        public void Visit(BinaryExpression assvd) {
            assvd.Left.Handle(this);
            assvd.Right.Handle(this);
        }
        public void Visit(FunctionCallNode funcall) {
            funcall.Name.Handle(this);
            //if (syntax.AllKeys.Is(nameNode.NameLex.Value)) {
            funcall.Args.ForEach(x => x.Handle(this));
        }

        void HighlightType(TypeNode dt) {
            if (dt.ModificationLex != null) {
                ApplyForegroundValue(dt.ModificationLex, KeywordBrush);
            }
            ApplyForegroundValue(dt.TypeLex, KeywordBrush);
        }

        static void ApplyForegroundValue(LexerNode lexer) {
            var tp = lexer.StartPointer;
            var range = new TextRange(tp, tp.GetPositionAtOffset(lexer.Value.Length));

            Brush brush;
            switch (lexer.Token) {
                case LexerTokens.Number:
                    brush = NumereticBrush;
                    break;
                case LexerTokens.String:
                    brush = StringBrush;
                    break;
                default:
                    return;
            }

            range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
        }

        static void ApplyForegroundValue(LexerNode lexer, Brush brush) {
            GetRange(lexer).ApplyPropertyValue(TextElement.ForegroundProperty, brush);
        }

        static void ApplyBackgroundValue(LexerNode lexer, Brush brush) {
            GetRange(lexer).ApplyPropertyValue(TextElement.BackgroundProperty, brush);
        }

        static void ApplyFontWeight(LexerNode lexer, FontWeight ft) {
            GetRange(lexer).ApplyPropertyValue(TextElement.FontWeightProperty, ft);
        }

        static TextRange GetRange(LexerNode lexer) {
            var tp = lexer.StartPointer;
            return new TextRange(tp, tp.GetTextPointAt(lexer.Value.Length));// tp.GetPositionAtOffset(lexer.Value.Length));
        }

        


    }

    public static class TextPointerEx {
        public static TextPointer GetTextPointAt(this TextPointer from, int pos) {
            TextPointer ret = from;
            int i = 0;

            while ((i < pos) && (ret != null)) {
                if ((ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) ||
                    (ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)) {
                    i++;
                }

                if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null) {
                    return ret;
                }

                ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
            }

            return ret;
        }
    }
}
