using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Veldrid;

namespace D3DLab.Std.Engine.Helpers {
    public static class AssetHelper {
        private static readonly string s_assetRoot = Path.Combine(AppContext.BaseDirectory, "Assets");

        internal static string GetPath(string assetPath) {
            return Path.Combine(s_assetRoot, assetPath);
        }
    }
    public static class ShaderHelper {
        public static Shader LoadShader(ResourceFactory factory, string setName, string stage, string entryPoint) {
            return LoadShader(factory,setName,(ShaderStages)Enum.Parse(typeof(ShaderStages),stage), entryPoint);
        }
        public static Shader LoadShader(ResourceFactory factory, string setName, ShaderStages stage, string entryPoint) {
            Shader shader = factory.CreateShader(new ShaderDescription(stage, LoadBytecode(factory, setName, stage), entryPoint));
            shader.Name = $"{setName}-{stage.ToString()}";
            return shader;
        }

        private static byte[] LoadBytecode(ResourceFactory factory, string setName, ShaderStages stage) {
            string name = setName + "-" + stage.ToString().ToLower();
            GraphicsBackend backend = factory.BackendType;

            if (backend == GraphicsBackend.Vulkan || backend == GraphicsBackend.Direct3D11) {
                string bytecodeExtension = GetBytecodeExtension(backend);
                string bytecodePath = AssetHelper.GetPath(Path.Combine("Shaders.Generated", name + bytecodeExtension));
                if (File.Exists(bytecodePath)) {
                    return File.ReadAllBytes(bytecodePath);
                }
            }

            string extension = GetSourceExtension(backend);
            string path = AssetHelper.GetPath(Path.Combine("Shaders.Generated", name + extension));
            return File.ReadAllBytes(path);
        }

        private static string GetBytecodeExtension(GraphicsBackend backend) {
            switch (backend) {
                case GraphicsBackend.Direct3D11:
                    return ".hlsl.bytes";
                case GraphicsBackend.Vulkan:
                    return ".450.glsl.spv";
                case GraphicsBackend.OpenGL:
                    throw new InvalidOperationException("OpenGL and OpenGLES do not support shader bytecode.");
                default:
                    throw new InvalidOperationException("Invalid Graphics backend: " + backend);
            }
        }

        private static string GetSourceExtension(GraphicsBackend backend) {
            switch (backend) {
                case GraphicsBackend.Direct3D11:
                    return ".hlsl";
                case GraphicsBackend.Vulkan:
                    return ".450.glsl";
                case GraphicsBackend.OpenGL:
                    return ".330.glsl";
                case GraphicsBackend.Metal:
                    return ".metallib";
                default:
                    throw new InvalidOperationException("Invalid Graphics backend: " + backend);
            }
        }
    }
}
