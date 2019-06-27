using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Veldrid.Utilities
{
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public Vector3 Minimum;
        public Vector3 Maximum;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Minimum = min;
            Maximum = max;
        }

        public ContainmentType Contains(ref BoundingBox other)
        {
            if (Maximum.X < other.Minimum.X || Minimum.X > other.Maximum.X
                || Maximum.Y < other.Minimum.Y || Minimum.Y > other.Maximum.Y
                || Maximum.Z < other.Minimum.Z || Minimum.Z > other.Minimum.Z)
            {
                return ContainmentType.Disjoint;
            }
            else if (Minimum.X <= other.Minimum.X && Maximum.X >= other.Maximum.X
                && Minimum.Y <= other.Minimum.Y && Maximum.Y >= other.Maximum.Y
                && Minimum.Z <= other.Minimum.Z && Maximum.Z >= other.Maximum.Z)
            {
                return ContainmentType.Contains;
            }
            else
            {
                return ContainmentType.Intersects;
            }
        }

        public Vector3 GetCenter()
        {
            return (Maximum + Minimum) / 2f;
        }

        public Vector3 GetDimensions()
        {
            return Maximum - Minimum;
        }

        public static unsafe BoundingBox Transform(BoundingBox box, Matrix4x4 mat)
        {
            AlignedBoxCorners corners = box.GetCorners();
            Vector3* cornersPtr = (Vector3*)&corners;

            Vector3 min = Vector3.Transform(cornersPtr[0], mat);
            Vector3 max = Vector3.Transform(cornersPtr[0], mat);

            for (int i = 1; i < 8; i++)
            {
                min = Vector3.Min(min, Vector3.Transform(cornersPtr[i], mat));
                max = Vector3.Max(max, Vector3.Transform(cornersPtr[i], mat));
            }

            return new BoundingBox(min, max);
        }

        public static unsafe BoundingBox CreateFromVertices(
            Vector3* vertices,
            int numVertices,
            Quaternion rotation,
            Vector3 offset,
            Vector3 scale)
            => CreateFromPoints(vertices, Unsafe.SizeOf<Vector3>(), numVertices, rotation, offset, scale);
        public static unsafe BoundingBox CreateFromPoints(
            Vector3* vertexPtr,
            int numVertices,
            int vertexStride,
            Quaternion rotation,
            Vector3 offset,
            Vector3 scale)
        {
            byte* bytePtr = (byte*)vertexPtr;
            Vector3 min = Vector3.Transform(*vertexPtr, rotation);
            Vector3 max = Vector3.Transform(*vertexPtr, rotation);

            for (int i = 1; i < numVertices; i++)
            {
                bytePtr = bytePtr + vertexStride;
                vertexPtr = (Vector3*)bytePtr;
                Vector3 pos = Vector3.Transform(*vertexPtr, rotation);

                if (min.X > pos.X) min.X = pos.X;
                if (max.X < pos.X) max.X = pos.X;

                if (min.Y > pos.Y) min.Y = pos.Y;
                if (max.Y < pos.Y) max.Y = pos.Y;

                if (min.Z > pos.Z) min.Z = pos.Z;
                if (max.Z < pos.Z) max.Z = pos.Z;
            }

            return new BoundingBox((min * scale) + offset, (max * scale) + offset);
        }

        public static unsafe BoundingBox CreateFromVertices(Vector3[] vertices)
        {
            return CreateFromVertices(vertices, Quaternion.Identity, Vector3.Zero, Vector3.One);
        }

        public static unsafe BoundingBox CreateFromVertices(Vector3[] vertices, Quaternion rotation, Vector3 offset, Vector3 scale)
        {
            Vector3 min = Vector3.Transform(vertices[0], rotation);
            Vector3 max = Vector3.Transform(vertices[0], rotation);

            for (int i = 1; i < vertices.Length; i++)
            {
                Vector3 pos = Vector3.Transform(vertices[i], rotation);

                if (min.X > pos.X) min.X = pos.X;
                if (max.X < pos.X) max.X = pos.X;

                if (min.Y > pos.Y) min.Y = pos.Y;
                if (max.Y < pos.Y) max.Y = pos.Y;

                if (min.Z > pos.Z) min.Z = pos.Z;
                if (max.Z < pos.Z) max.Z = pos.Z;
            }

            return new BoundingBox((min * scale) + offset, (max * scale) + offset);
        }

        public static BoundingBox Combine(BoundingBox box1, BoundingBox box2)
        {
            return new BoundingBox(
                Vector3.Min(box1.Minimum, box2.Minimum),
                Vector3.Max(box1.Maximum, box2.Maximum));
        }

        public static bool operator ==(BoundingBox first, BoundingBox second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(BoundingBox first, BoundingBox second)
        {
            return !first.Equals(second);
        }

        public bool Equals(BoundingBox other)
        {
            return Minimum == other.Minimum && Maximum == other.Maximum;
        }

        public override string ToString()
        {
            return string.Format("Min:{0}, Max:{1}", Minimum, Maximum);
        }

        public override bool Equals(object obj)
        {
            return obj is BoundingBox && ((BoundingBox)obj).Equals(this);
        }

        public override int GetHashCode()
        {
            int h1 = Minimum.GetHashCode();
            int h2 = Maximum.GetHashCode();
            uint shift5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)shift5 + h1) ^ h2;
        }

        public AlignedBoxCorners GetCorners()
        {
            AlignedBoxCorners corners;
            corners.NearBottomLeft = new Vector3(Minimum.X, Minimum.Y, Maximum.Z);
            corners.NearBottomRight = new Vector3(Maximum.X, Minimum.Y, Maximum.Z);
            corners.NearTopLeft = new Vector3(Minimum.X, Maximum.Y, Maximum.Z);
            corners.NearTopRight = new Vector3(Maximum.X, Maximum.Y, Maximum.Z);

            corners.FarBottomLeft = new Vector3(Minimum.X, Minimum.Y, Minimum.Z);
            corners.FarBottomRight = new Vector3(Maximum.X, Minimum.Y, Minimum.Z);
            corners.FarTopLeft = new Vector3(Minimum.X, Maximum.Y, Minimum.Z);
            corners.FarTopRight = new Vector3(Maximum.X, Maximum.Y, Minimum.Z);

            return corners;
        }

        public bool ContainsNaN()
        {
            return float.IsNaN(Minimum.X) || float.IsNaN(Minimum.Y) || float.IsNaN(Minimum.Z)
                || float.IsNaN(Maximum.X) || float.IsNaN(Maximum.Y) || float.IsNaN(Maximum.Z);
        }

    }
}