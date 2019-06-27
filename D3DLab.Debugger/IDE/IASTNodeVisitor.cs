using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3DLab.Debugger.IDE {
    interface IASTNodeVisitor {
        //combined
        void Visit(CommentsNode node);
        void Visit(IncludeDeclaration includeDeclaration);
        void Visit(FunctionDeclarationNode definitionObjectNode);
        void Visit(VarDeclaration varDeclaration);

        //containers
        void Visit(DefinitionObjectNode definitionObjectNode);
        void Visit(VariableDefinitionWitSemanticName definitionObjectNode);
        void Visit(CBufferNode definitionObjectNode);
        void Visit(StructNode definitionObjectNode);

        //names
        void Visit(NameNode nameNode);
        void Visit(ArrayNameNode nameNode);
        void Visit(StringNode definitionObjectNode);
        void Visit(NumbertNode definitionObjectNode);
        void Visit(SystemName definitionObjectNode);


        //types
        void Visit(GenericTypeNode typeNode);
        void Visit(DatatypeNode typeNode);
        void Visit(PrimitiveType typeNode);
        void Visit(ArrayType typeNode);
        void Visit(StringTypeNode typeNode);

        //eexpressions
        void Visit(BinaryExpression binaryExpression);
        void Visit(FunctionCallNode functionCallNode);

    }
}
