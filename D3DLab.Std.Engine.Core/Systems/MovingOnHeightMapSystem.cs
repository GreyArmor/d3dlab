using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Movements;
using D3DLab.Std.Engine.Core.Ext;
using D3DLab.Std.Engine.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace D3DLab.Std.Engine.Core.Systems {
    public interface IStickOnHeightMapComponent : IGraphicComponent {
        Vector3 AxisUpLocal { get; }
        Vector3 AttachPointLocal { get; }
    }
    public interface IHeightMapSourceComponent : IGraphicComponent {
        Matrix4x4 GetTransfromToMap(ref Ray ray);
    }
    public class MovingOnHeightMapSystem : BaseEntitySystem, IGraphicSystem {

        protected override void Executing(SceneSnapshot snapshot) {
            var emanager = snapshot.ContextState.GetEntityManager();
            var toProcess = new List<GraphicEntity>();
            IHeightMapSourceComponent source = null;
            foreach (var entity in emanager.GetEntities()) {
                if (entity.Has<IStickOnHeightMapComponent>()) {
                    toProcess.Add(entity);
                }
                var s = entity.GetComponents<IHeightMapSourceComponent>();
                if (s.Any()) {
                    source = s.First();
                }
            }

            if (source.IsNull()) {
                return;
            }

            foreach(var en in toProcess) {
                var com = en.GetComponent<IStickOnHeightMapComponent>();
                var box = en.GetComponent<IGeometryComponent>().Box;
                var tr = en.GetComponent<TransformComponent>();

                var rayLocal = new Ray(com.AttachPointLocal, com.AxisUpLocal);

                var rayW = rayLocal.Transformed(tr.MatrixWorld);

                var matrix = source.GetTransfromToMap(ref rayW);

                tr.MatrixWorld *= matrix;
            }

        }
    }
}
