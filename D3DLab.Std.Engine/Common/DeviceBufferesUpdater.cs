using D3DLab.Std.Engine.Core.Shaders;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine.Common {
    public abstract class DeviceBufferesUpdater {
        protected DeviceBuffer indexBuffer;
        DeviceBuffer worldBuffer;
        protected DeviceBuffer vertexBuffer;

        public DeviceBuffer World { get => worldBuffer; }
        public DeviceBuffer Index { get => indexBuffer; }
        public DeviceBuffer Vertex { get => vertexBuffer; }

        protected DisposeCollectorResourceFactory factory;
        protected CommandList cmd;
        protected readonly IVeldridShaderSpecification shader;

        public DeviceBufferesUpdater(IVeldridShaderSpecification shader) {
            this.shader = shader;
        }
        public void Update(DisposeCollectorResourceFactory factory, CommandList commands) {
            this.factory = factory;
            this.cmd = commands;
        }

        public void UpdateWorld() {
            factory.CreateIfNullBuffer(ref worldBuffer, new BufferDescription(64, BufferUsage.UniformBuffer));
            cmd.UpdateBuffer(worldBuffer, 0, Matrix4x4.Identity);
        }

        public abstract void UpdateVertex(Geometry3D geo);
        public void UpdateIndex(Geometry3D geo) {
            var indices = shader.ConvertToShaderIndices(geo);
            factory.CreateIfNullBuffer(ref indexBuffer, new BufferDescription(sizeof(ushort) * (uint)indices.Length,
                 BufferUsage.IndexBuffer));
            cmd.UpdateBuffer(indexBuffer, 0, indices);
        }
    }
}
