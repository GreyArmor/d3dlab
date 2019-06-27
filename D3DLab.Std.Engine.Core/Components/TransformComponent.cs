using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace D3DLab.Std.Engine.Core.Components {
    public class TransformComponent : GraphicComponent {
        private Matrix4x4 matrixWorld;
        public virtual Matrix4x4 MatrixWorld {
            get => matrixWorld;
            set {
                matrixWorld = value;
                IsModified = true;
            }
        }
        public TransformComponent() {
            MatrixWorld = Matrix4x4.Identity;
        }
    }
}
