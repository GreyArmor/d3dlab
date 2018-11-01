﻿using D3DLab.Std.Engine.Core.Shaders;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace D3DLab.SDX.Engine.Shader {
    [Flags]
    internal enum ShaderStages : byte {
        /// <summary>
        /// No stages.
        /// </summary>
        None = 0,
        /// <summary>
        /// The vertex shader stage.
        /// </summary>
        Vertex = 1 << 0,
        /// <summary>
        /// The geometry shader stage.
        /// </summary>
        Geometry = 1 << 1,
        /// <summary>
        /// The tessellation control (or hull) shader stage.
        /// </summary>
        TessellationControl = 1 << 2,
        /// <summary>
        /// The tessellation evaluation (or domain) shader stage.
        /// </summary>
        TessellationEvaluation = 1 << 3,
        /// <summary>
        /// The fragment (or pixel) shader stage.
        /// </summary>
        Fragment = 1 << 4,
        /// <summary>
        /// The compute shader stage.
        /// </summary>
        Compute = 1 << 5,
    }

    internal class D3DInclude : Include {
        Stream stream;
        readonly Dictionary<string, string> resources;

        public D3DInclude(Dictionary<string, string> resources) {
            this.resources = resources;
        }

        public void Close(Stream stream) {
            stream.Close();
        }

        //file name - #include "./Shaders/Common.fx"
        public Stream Open(IncludeType type, string fileName, Stream parentStream) {
            var key = Path.GetFileNameWithoutExtension(fileName);
            stream = this.GetType().Assembly.GetManifestResourceStream(resources[key]);

            return stream;
        }

        public IDisposable Shadow {
            get {
                return this.stream;
            }
            set {
                if (this.stream != null) {
                    this.stream.Dispose();
                }

                this.stream = value as Stream;
            }
        }

        public void Dispose() {
            stream.Dispose();
        }
    }

    internal class D3D11Compilator {
        readonly ShaderFlags sFlags = ShaderFlags.None;
        readonly EffectFlags eFlags = EffectFlags.None;

        public string Preprocess(string shadertext, Include include) {
            return ShaderBytecode.Preprocess(shadertext, new ShaderMacro[0], include);
        }

        internal byte[] Compile(byte[] shader, string entrypoint, ShaderStages stage, string name) { // vs_5_0  ps_5_0
            //fxc /E VS /T vs_5_0 Vertex.hlsl /Fo Vertex.hlsl.bytes
            string profile = null;
            switch (stage) {
                case ShaderStages.Vertex:
                    profile = "vs_5_0";
                    break;
                case ShaderStages.Geometry:
                    profile = "gs_5_0";
                    break;
                case ShaderStages.TessellationControl:
                    profile = "hs_5_0";
                    break;
                case ShaderStages.TessellationEvaluation:
                    profile = "ds_5_0";
                    break;
                case ShaderStages.Fragment:
                    profile = "ps_5_0";
                    break;
                case ShaderStages.Compute:
                    profile = "cs_5_0";
                    break;

            }
            
            using (var res = ShaderBytecode.Compile(shader, entrypoint, profile, sFlags, eFlags, name)) {
                if (res.Bytecode == null) {
                    throw new Exception(res.Message);
                }
                return res.Bytecode.Data;
            }
        }
        //internal void CompileToFile(FileInfo file, string shadertext) {
        //    var shaderBytes = ShaderBytecode.Compile(shadertext, "vs_5_0", sFlags, eFlags);
        //    File.WriteAllBytes(file.FullName, shaderBytes.Bytecode.Data);
        //}
    }

    public class D3DShaderCompilator : IShaderCompilator {
        readonly D3D11Compilator compilator;
        readonly Dictionary<string, string> resources;

        public D3DShaderCompilator() {
            compilator = new D3D11Compilator();
            resources = new Dictionary<string, string>();
        }

        public void CompileWithPreprocessing(IShaderInfo info) {
            var text = info.ReadText();
            CompileWithPreprocessing(info, text);
        }
        public void CompileWithPreprocessing(IShaderInfo info, string text) {
            info.ReadText();
            var preprocessed = compilator.Preprocess(text, new D3DInclude(resources));
            var bytes = Encoding.UTF8.GetBytes(preprocessed);
            bytes = compilator.Compile(bytes, info.EntryPoint, ConvertToShaderStage(info.Stage), info.Name);
            info.WriteCompiledBytes(bytes);
        }

        public void Compile(IShaderInfo info) {
            var bytes = info.ReadBytes();
            bytes = compilator.Compile(bytes, info.EntryPoint, ConvertToShaderStage(info.Stage), info.Name);
            info.WriteCompiledBytes(bytes);
        }

        public void Compile(IShaderInfo info, string text) {
            var bytes = Encoding.UTF8.GetBytes(text);
            bytes = compilator.Compile(bytes, info.EntryPoint, ConvertToShaderStage(info.Stage), info.Name);
            info.WriteCompiledBytes(bytes);
        }

        public byte[] Compile(string text, string entryPoint, string stage) {
            var bytes = Encoding.UTF8.GetBytes(text);
            return compilator.Compile(bytes, entryPoint, ConvertToShaderStage(stage), "undefined");
        }

        private static ShaderStages ConvertToShaderStage(string stage) {
            return (ShaderStages)Enum.Parse(typeof(ShaderStages), stage);
        }

        internal void AddIncludeMapping(string include, string resource) {
            resources.Add(include, resource);
        }
    }
}
