using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using D3DLab.Std.Engine.Core.MeshFormats;
using g3;

namespace D3DLab.Std.Engine.Core.Common {
    public abstract class ReadOnlyList<T> where T : struct {
        public T this[int index] {
            get { return array[index]; }
            set { array[index] = value; }
        }
        public int Count => array.Length;

        readonly T[] array;
        protected ReadOnlyList(T[] vectors) {
            this.array = vectors;
        }
        protected ReadOnlyList(int length) {
            this.array = new T[length];
        }

        public IEnumerator<T> GetEnumerator() {
            return array.ToList().GetEnumerator();
        }

        public static implicit operator List<T>(ReadOnlyList<T> d) {
            return d.array.ToList();
        }
    }
    public class Vector4Collection : ReadOnlyList<Vector4> {
        public Vector4Collection(List<Vector4> vectors) : base(vectors.ToArray()) {
        }
    }
    public class Vector3Collection : ReadOnlyList<Vector3> {
        public Vector3Collection(List<Vector3> vectors) : base(vectors.ToArray()) { }
        public Vector3Collection(int length) : base(length) { }
    }
    public class Vector2Collection : ReadOnlyList<Vector2> {
        public Vector2Collection(List<Vector2> vectors) : base(vectors.ToArray()) {
        }
    }
    public class IntCollection : ReadOnlyList<int> {
        public IntCollection(List<int> vectors) : base(vectors.ToArray()) {
        }
    }

    public class GroupGeometry3D {
        public IReadOnlyCollection<PartGeometry3D> Parts => groupsmap.Values;
        public IReadOnlyCollection<Vector3> Positions => mapper.Keys;
        public IReadOnlyCollection<int> Indices => indices.AsReadOnly();

        internal readonly Dictionary<Vector3, int> mapper;
        internal readonly Dictionary<int, PartGeometry3D> posIndexToPartMapper;
        readonly Dictionary<string, PartGeometry3D> groupsmap;
        public readonly List<Vector3> positions;
        public readonly List<int> indices;

        public GroupGeometry3D() {
            posIndexToPartMapper = new Dictionary<int, PartGeometry3D>();
            mapper = new Dictionary<Vector3, int>();
            positions = new List<Vector3>();
            indices = new List<int>();
            groupsmap = new Dictionary<string, PartGeometry3D>();
        }

        public PartGeometry3D CreatePart(string fullName) {
            if (!groupsmap.TryGetValue(fullName, out var p)) {
                p = new PartGeometry3D(this, fullName);
                groupsmap.Add(fullName, p);
            }
            p.Groups.Add(new ObjGroup(fullName));
            return p;
        }

        public void Fixed() {
            posIndexToPartMapper.Clear();
            mapper.Clear();
            foreach (var kv in groupsmap.Values.ToList()) {
                if (kv.IsEmpty) {
                    groupsmap.Remove(kv.Name);
                }
            }
        }

    }



    public class PartGeometry3D {
        public IReadOnlyCollection<Vector3> Positions => mapper.Keys;
        public IReadOnlyCollection<int> Indices => indices.AsReadOnly();

        public bool IsEmpty => mapper.Count == 0 && indices.Count == 0;

        readonly List<int> indices;
        public readonly Dictionary<Vector3, int> mapper;
        public readonly GroupGeometry3D full;
        public readonly string Name;

        public readonly List<ObjGroup> Groups;

        public PartGeometry3D(GroupGeometry3D group, string key) {
            mapper = new Dictionary<Vector3, int>();
            indices = new List<int>();
            this.full = group;
            this.Name = key;
            Groups = new List<ObjGroup>();
        }

        public void AddPosition(ref Vector3 v) {
            var group = Groups.Last();
            var index = full.mapper.Count;

            if (full.mapper.ContainsKey(v)) {
                index = full.mapper[v];
            } else {
                full.mapper.Add(v, index);
            }
            full.positions.Add(v);

            if (group.PosGroupInfo == null) {
                group.PosGroupInfo = new ObjGroupInfo {
                    Name = group.Name,
                    StartIndex = index,
                };
            }

            group.PosGroupInfo.Count += 1;
        }

        public void AddTriangle(IList<int> ind) {
            var newindexes = new HashSet<int>();
            for (var i = 0; i < ind.Count; ++i) {
                var index = ind[i];
                var v = full.positions[index];
                newindexes.Add(full.mapper[v]);
            }

            if(newindexes.Count < 3) {// triangle is not triangle ... it is line, skip it
                throw new Exception($"Triangle is not triangle ... it is line [{string.Join(",", ind)}]");
            }

            var group = Groups.Last();
            for (var i = 0; i < ind.Count; ++i) {
                try {
                    var index = ind[i];
                    var v = full.positions[index];
                    index = full.mapper[v];

                    if (!mapper.TryGetValue(v, out var partIndex)) {
                        partIndex = mapper.Count;
                        mapper.Add(v, partIndex);
                    }

                    if (group.IndxGroupInfo == null) {
                        group.IndxGroupInfo = new ObjGroupInfo {
                            Name = group.Name,
                            StartIndex = index,
                            Count = 1
                        };
                    } else {
                        group.IndxGroupInfo.Count += 1;
                    }
                    full.indices.Add(index);
                    indices.Add(partIndex);
                } catch (Exception ex) {
                    ex.ToString();
                }
            }
        }
    }

    public class AbstractGeometry3D {
        public List<Vector3> Positions { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Vector3> Tangents { get; set; }
        public List<Vector3> BiTangents { get; set; }
        public List<Vector4> Colors { get; set; }
        public List<Vector2> TextureCoordinates { get; set; }
        public List<int> Indices { get; set; }

        public Vector4 Color { get; set; }



        public AbstractGeometry3D() {
            Colors = new List<Vector4>();
            Positions = new List<Vector3>();
            Normals = new List<Vector3>();
            TextureCoordinates = new List<Vector2>();
            Indices = new List<int>();
        }


        public override string ToString() {
            return $"[Positions:{Positions.Count};Indices:{Indices.Count}]";
        }



    }
}
