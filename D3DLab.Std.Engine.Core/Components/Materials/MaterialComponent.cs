using System.IO;
using System.Numerics;

namespace D3DLab.Std.Engine.Core.Components.Materials {
       
    public abstract class MaterialComponent : GraphicComponent {

    }

    public class GradientMaterialComponent : MaterialComponent {
        public Vector4 Apex { get; set; }
        public Vector4 Center { get; set; }

        public GradientMaterialComponent() {
            // Set the Pink color at the top of the sky dome.
            Apex = new Vector4(0.0f, 0.15f, 0.66f, 1.0f); ;
            // Set the Blue color at the center of the sky dome.
            Center = new Vector4(0.81f, 0.38f, 0.66f, 1.0f);
            IsModified = true;
        }
    }

    public interface IColoringMaterialComponent : IGraphicComponent {
        Vector4 GetVertexColor(int vertexInd);
    }

    public class ColorComponent : MaterialComponent, IColoringMaterialComponent {
        private Vector4 color;
        public Vector4 Color {
            get => color;
            set {
                color = value;
                IsModified = true;
            }
        }

        public Vector4 GetVertexColor(int vertexInd) {
            throw new System.NotImplementedException();
        }
    }
    

    public class MaterialComponent1 : GraphicComponent {
        public float Specular { get; set; } = -1; // -1 not specular

        public Vector4 AmbientColor { get; set; }
        public Vector4 DiffuseColor { get; set; }

    }

    public class TexturedMaterialComponent : MaterialComponent {
        /// <summary>
        /// Order is important! the same order will be setted in shader recources
        /// </summary>
        public FileInfo[] Images { get; }

        public TexturedMaterialComponent(params FileInfo[] image) {
            Images = image;
        }


    }
}
