using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DLab.Fwk.ImageSharp
{
    public static class ImagePr {
        public static Image<Rgba32> Load(string path) {
            using (var str = System.IO.File.OpenRead(path)) {
               return Image.Load(str);
            }
        }
    }
}
