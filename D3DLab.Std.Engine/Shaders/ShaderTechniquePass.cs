using D3DLab.Std.Engine.Core.Shaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veldrid;

namespace D3DLab.Std.Engine.Shaders {
    /// <summary>
    /// Class just describe shader technique structure, no spetific actions or behaviours just for readability
    /// </summary>
    public class ShaderTechniquePass : IRenderTechniquePass {
        public bool IsCached { get; private set; }

        public IShaderInfo VertexShader { get => Get(Veldrid.ShaderStages.Vertex); }
        public IShaderInfo GeometryShader { get => Get(Veldrid.ShaderStages.Geometry); }
        public IShaderInfo PixelShader { get => Get(Veldrid.ShaderStages.Fragment); }

        public ShaderSetDescription Description { get; private set; }

        public IShaderInfo[] ShaderInfos { get; }

        readonly Dictionary<Veldrid.ShaderStages, IShaderInfo> pass;
        readonly Dictionary<Veldrid.ShaderStages, Shader> shaders;

        public ShaderTechniquePass(IShaderInfo[] shaderInfos) {
            this.ShaderInfos = shaderInfos;
            pass = new Dictionary<Veldrid.ShaderStages, IShaderInfo>();
            foreach (var info in shaderInfos) {
                var stage = (Veldrid.ShaderStages)Enum.Parse(typeof(Veldrid.ShaderStages), info.Stage);
                if (pass.ContainsKey(stage)) {
                    throw new Exception($"One pass can contain only one {stage} shader");
                }
                pass.Add(stage, null);
            }
        }

        Shader[] ToArray() {
            return shaders.Values.ToArray();
        }
        public void ClearCache() {
            pass.Clear();
            IsCached = false;
        }

        public void Update(ResourceFactory factory, VertexLayoutDescription[] layoutDescriptions) {
            pass.Clear();
            foreach (var info in ShaderInfos) {
                var stage = (Veldrid.ShaderStages)Enum.Parse(typeof(Veldrid.ShaderStages), info.Stage);
                Shader shader = factory.CreateShader(new ShaderDescription(stage, info.ReadCompiledBytes(), info.EntryPoint));
                shader.Name = Path.GetFileName(info.Name);
                shaders[stage] = shader;
                pass[stage] = info;
            }
            Description = new ShaderSetDescription(layoutDescriptions, ToArray());
            IsCached = true;
        }

        IShaderInfo Get(Veldrid.ShaderStages stage) {
            return pass[stage];
        }
    }
}
