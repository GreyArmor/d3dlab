using D3DLab.Std.Engine.Core.Shaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Veldrid;

namespace D3DLab.Std.Engine {   

    public struct D3DShaderInfo : IShaderInfo {
        const string extention = ".hlsl";
        const string binary_extention = ".hlsl.bytes";

        public string Name { get { return compiledPath; } }

        /// <summary>
        /// Vertex/Fragment
        /// </summary>
        public string Stage { get; }
        public string EntryPoint { get; }
        readonly string path;
        readonly string compiledPath;

        public D3DShaderInfo(string directory, string filename, string stage, string entry) {
            path = System.IO.Path.Combine(directory, filename + extention);
            compiledPath = System.IO.Path.ChangeExtension(path, binary_extention);
            Stage = stage;
            EntryPoint = entry;
        }

        public byte[] ReadCompiledBytes() {
            return File.ReadAllBytes(compiledPath);
        }

        public FileInfo GetFileInfo() {
            return new FileInfo(path);
        }

        public string ReadText() {
            return File.ReadAllText(path);
        }

        public byte[] ReadBytes() {
            return File.ReadAllBytes(path);
        }

        public void WriteCompiledBytes(byte[] bytes) {
            File.WriteAllBytes(compiledPath, bytes);
        }
    }

    public class D3DShaderCompilator : IShaderCompilator {
        readonly Veldrid.D3D11.D3D11ShaderCompilator compilator;

        public IEnumerable<IShaderInfo> Infos { get; }
        public D3DShaderCompilator(IEnumerable<IShaderInfo> infos) {
            Infos = infos;
            compilator = new Veldrid.D3D11.D3D11ShaderCompilator();
        }
        public void Compile(IShaderInfo info) {
            var bytes = info.ReadBytes();
            bytes = compilator.Compile(bytes, info.EntryPoint, ConvertToShaderStage(info.Stage));
            info.WriteCompiledBytes(bytes);
        }

        public void Compile(IShaderInfo info, string text) {
            var bytes = Encoding.UTF8.GetBytes(text);
            bytes = compilator.Compile(bytes, info.EntryPoint, ConvertToShaderStage(info.Stage));
            info.WriteCompiledBytes(bytes);
        }

        public byte[] Compile(string text, string entryPoint, string stage) {
            var bytes = Encoding.UTF8.GetBytes(text);
            return compilator.Compile(bytes, entryPoint, ConvertToShaderStage(stage));
        }

        private static ShaderStages ConvertToShaderStage(string stage) {
            return (ShaderStages)Enum.Parse(typeof(ShaderStages), stage);
        }
    }
}
