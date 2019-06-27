using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace D3DLab {

    public class CncPath {
        public List<Vector3> Points;

    }

    public class CncParser {
        readonly FileInfo file;

        public CncParser(FileInfo file) {
            this.file = file;
        }

        public CncPath[] GetPaths() {
            var p = new List<CncPath>();
            CncPath path = null;
            foreach (var line in File.ReadAllLines(file.FullName)) {
                //G1 Y=0.527 X=17.715 Z=7.105 V=13.0

                if(line.StartsWith("T", StringComparison.InvariantCultureIgnoreCase)) {
                    if (path != null) {
                        p.Add(path);
                    }
                    path = new CncPath { Points = new List<Vector3>() };
                    continue;
                }

                if (!line.StartsWith("G", StringComparison.InvariantCultureIgnoreCase)) { continue; }

                var parts = line.Split(' ');

                var v = new Vector3();
                for (int i = 0; i < parts.Length; i++) {
                    var part = parts[i];
                    switch (part[0]) {
                        case 'X':
                            v.X = float.Parse(part.Remove(0, 2));
                            break;
                        case 'Y':
                            v.Y = float.Parse(part.Remove(0, 2));
                            break;
                        case 'Z':
                            v.Z = float.Parse(part.Remove(0, 2));
                            break;
                    }
                }
                path.Points.Add(v);
            }

            p.Add(path);

            return p.ToArray();
        }
    }
}
