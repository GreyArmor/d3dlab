using D3DLab.Std.Engine.Core.Shaders;
using D3DLab.Std.Engine.Shaders;
using System;
using System.Linq;
using Veldrid;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine.Common {
    public interface IVeldridShaderSpecification {
        ShaderTechniquePass[] passes { get; }

        VertexLayoutDescription[] GetVertexDescription();
        ResourceLayoutDescription GetResourceDescription();
        uint GetVertexSizeInBytes();

        void ReLoad();
        void UpdateShaders(DisposeCollectorResourceFactory factory);
        ushort[] ConvertToShaderIndices(Geometry3D geo);
        TVertex[] ConvertVertexToShaderStructure<TVertex>(Geometry3D geo) where TVertex : struct;
    }

    public abstract class ShaderSpecification<TVertex> : IVeldridShaderSpecification, IRenderTechnique where TVertex : struct {

        public IRenderTechniquePass[] Passes => passes;
        public ShaderTechniquePass[] passes { get; }
        public ShaderSpecification(ShaderTechniquePass[] passes) {
            this.passes = passes;
        }

        public abstract VertexLayoutDescription[] GetVertexDescription();
        public abstract ResourceLayoutDescription GetResourceDescription();

        public abstract uint GetVertexSizeInBytes();

        public void ReLoad() {
            Passes[0].ClearCache();
        }

        public void UpdateShaders(DisposeCollectorResourceFactory factory) {
            if (passes.All(x => x.IsCached)) {
                return;
            }
            foreach (var pass in passes) {
                pass.Update(factory, GetVertexDescription());
            }
        }

        public ushort[] ConvertToShaderIndices(Geometry3D geo) {
            return geo.Indices.Select(x => (ushort)x).ToArray();
        }

        protected abstract TVertex[] ConvertVertexToShaderStructure(Geometry3D geo);

        public T[] ConvertVertexToShaderStructure<T>(Geometry3D geo) where T : struct {
            if(this is ShaderSpecification<T> getter) {
                return getter.ConvertVertexToShaderStructure(geo);
            }
            return new T[0];

        }
    }
}
