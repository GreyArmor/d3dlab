using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using D3DLab.Std.Engine.Core.Common;

namespace D3DLab.Std.Engine.Core.Animation {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SkinningVertex {
        public uint BoneIndex0;
        public uint BoneIndex1;
        public uint BoneIndex2;
        public uint BoneIndex3;
        public float BoneWeight0;
        public float BoneWeight1;
        public float BoneWeight2;
        public float BoneWeight3;
    }

    /// <summary>
    /// Vertex input structure: Position, Normal and Color
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector4 Color;
        public Vector2 UV;
        public SkinningVertex Skin;

        /// <summary>
        /// Create vertex with position (normal will be based on position and color will be white)
        /// </summary>
        /// <param name="position">Vertex position</param>
        public Vertex(Vector3 position)
            : this(position, V4Colors.White) { }

        /// <summary>
        /// Create vertex with position and color (normal will be based on position)
        /// </summary>
        /// <param name="position">Vertex position</param>
        /// <param name="color">Vertex color</param>
        public Vertex(Vector3 position, Vector4 color)
            : this(position, Vector3.Normalize(position), color) { }

        /// <summary>
        /// Create vertex with position from individual components (normal will be calculated and color will be white)
        /// </summary>
        /// <param name="pX">X</param>
        /// <param name="pY">Y</param>
        /// <param name="pZ">Z</param>
        public Vertex(float pX, float pY, float pZ)
            : this(new Vector3(pX, pY, pZ)) { }

        /// <summary>
        /// Create vertex with position and color from individual components (normal will be calculated)
        /// </summary>
        /// <param name="pX">X</param>
        /// <param name="pY">Y</param>
        /// <param name="pZ">Z</param>
        /// <param name="color">color</param>
        public Vertex(float pX, float pY, float pZ, Vector4 color)
            : this(new Vector3(pX, pY, pZ), color) { }

        /// <summary>
        /// Create vertex with position, normal and color from individual components
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        /// <param name="nZ"></param>
        /// <param name="color"></param>
        public Vertex(float pX, float pY, float pZ, float nX, float nY, float nZ, Vector4 color)
            : this(new Vector3(pX, pY, pZ), new Vector3(nX, nY, nZ), color) { }

        /// <summary>
        /// Create vertex with position from individual components and normal and color
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pZ"></param>
        /// <param name="normal"></param>
        /// <param name="color"></param>
        public Vertex(float pX, float pY, float pZ, Vector3 normal, Vector4 color)
            : this(new Vector3(pX, pY, pZ), normal, color) { }

        /// <summary>
        /// Create vertex with position, normal and color - UV and Skin will be 0
        /// </summary>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        /// <param name="color"></param>
        public Vertex(Vector3 position, Vector3 normal, Vector4 color)
            : this(position, normal, color, Vector2.Zero, new SkinningVertex()) { }

        /// <summary>
        /// Create vertex with position, normal, color and uv coordinates
        /// </summary>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        /// <param name="color"></param>
        /// <param name="uv"></param>
        public Vertex(Vector3 position, Vector3 normal, Vector4 color, Vector2 uv)
            : this(position, normal, color, uv, new SkinningVertex()) { }

        /// <summary>
        /// Create vertex with position, normal, color, uv coordinates, and skinning
        /// </summary>
        /// <param name="position"></param>
        /// <param name="normal"></param>
        /// <param name="color"></param>
        public Vertex(Vector3 position, Vector3 normal, Vector4 color, Vector2 uv, SkinningVertex skin) {
            Position = new Vector4(position, 1);
            Normal = normal;
            Color = color;
            UV = uv;
            Skin = skin;
        }
    }
}
