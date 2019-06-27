using D3DLab.Std.Engine.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace D3DLab.Std.Engine.App
{
    public struct RenderOrderKey : IComparable<RenderOrderKey>, IComparable {
        public readonly ulong Value;

        public RenderOrderKey(ulong value) {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RenderOrderKey Create(int materialID, float cameraDistance)
            => Create((uint)materialID, cameraDistance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RenderOrderKey Create(uint materialID, float cameraDistance) {
            uint cameraDistanceInt = (uint)Math.Min(uint.MaxValue, (cameraDistance * 1000f));

            return new RenderOrderKey(
                ((ulong)materialID << 32) +
                cameraDistanceInt);
        }

        public int CompareTo(RenderOrderKey other) {
            return Value.CompareTo(other.Value);
        }

        int IComparable.CompareTo(object obj) {
            return Value.CompareTo(obj);
        }
    }

    public interface IOrderComponent {
        RenderOrderKey GetRenderOrderKey(Vector3 v);
    }

    internal class RenderOrderKeyComparer : IComparer<IOrderComponent> {
        public Vector3 CameraPosition { get; set; }
        public int Compare(IOrderComponent x, IOrderComponent y) {
            return x.GetRenderOrderKey(CameraPosition).CompareTo(y.GetRenderOrderKey(CameraPosition));
        }
    }
}
