using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3DLab.Debugger.IDE {
    class ShaderSyntaxInfo {
        public class Hash: HashSet<string> {
            public Hash() { }
            public Hash(IEnumerable<string> enu) : base(enu) { }

            public bool Is(string key) {
                return Contains(key);
            }
        }

        public const string StructKey = "struct";
        public const string CbufferKey = "cbuffer";

        public const string Semicolon = ";";
        public const string Colon = ":";
        public const string StartArrayDeclaration = "[";
        public const string StartFuncDeclaration = "(";
        public const string SequenceDeclaration = "{";

        public string[] VarArrayBoundary = { StartArrayDeclaration, "]" };

        public bool IsOperator(char _char) {
            return "+-*/%=&|<>!:;".IndexOf(_char) > 0;
        }

        public bool IsSemicolon(LexerNode lex) {
            return Semicolon == lex.Value;
        }

        public readonly Hash Objects = new Hash {
            "struct","cbuffer"
        };
        public readonly Hash Array = new Hash {
             "float2", "float3", "float4",
        };
        public readonly Hash Primitive = new Hash {
            "int", "uint", "return", "void", "bool", "float","true", "false", "discard", "break"
        };
        public readonly Hash Modifications = new Hash {
            "inout", "triangle", "noperspective",
            "line", "point", "lineadj" // geometry shader
        };
        public readonly Hash Generics = new Hash {
            "TriangleStream", "LineStream","StructuredBuffer"
        };
        public readonly Hash Matrix = new Hash {
            "float4x4",
        };
        public readonly Hash SystemFuncs = new Hash { "mul", "register", "dot","pow", "saturate", "abs" };

        public Hash AllKeys {
            get { return new Hash(Array
                    .Union(Primitive)
                    .Union(Matrix)
                    .Union(Objects)
                    .Union(Generics)
                    .Union(Modifications)
                    .Union(SystemFuncs)
                    );
            }
        }
    }
}
