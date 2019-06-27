using D3DLab.Std.Engine.Core.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DLab.SDX.Engine.Shader {
    /// <summary>
    /// Class just describe shader technique structure, no spetific actions or behaviours just for readability
    /// </summary>
    public class D3DShaderTechniquePass : IRenderTechniquePass {
        public IShaderInfo VertexShader { get => Get(ShaderStages.Vertex); }
        public IShaderInfo GeometryShader { get => Get(ShaderStages.Geometry); }
        public IShaderInfo PixelShader { get => Get(ShaderStages.Fragment); }

        public IShaderInfo[] ShaderInfos { get; }

        public bool IsCompiled { get; private set; }

        readonly Dictionary<ShaderStages, IShaderInfo> shaders;

        public D3DShaderTechniquePass(IShaderInfo[] shaderInfos) {
            this.ShaderInfos = shaderInfos;
            shaders = new Dictionary<ShaderStages, IShaderInfo>();
            foreach (var info in shaderInfos) {
                var stage = (ShaderStages)Enum.Parse(typeof(ShaderStages), info.Stage, true);
                if (shaders.ContainsKey(stage)) {
                    throw new Exception($"One pass can contain only one {stage} shader");
                }
                shaders.Add(stage, info);
            }
        }

        IShaderInfo Get(ShaderStages stage) {
            return shaders.ContainsKey(stage) ? shaders[stage] : null;
        }

        public void ClearCache() {
            IsCompiled = false;
        }

        public void Compile(IShaderCompilator compilator) {
            foreach(var sh in shaders) {
                compilator.CompileWithPreprocessing(sh.Value);
            }
            IsCompiled = true;
        }
    }
}
