using D3DLab.Std.Engine.Core.Components;
using D3DLab.Std.Engine.Core.Components.Materials;
using D3DLab.Std.Engine.Core.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace D3DLab.Std.Engine.Core.Systems {  
    public struct LightState {
        public float Intensity;
        public Vector3 Position;
        public Vector3 Direction;
        public LightTypes Type;
        public Vector4 Color;

        public LightStructBuffer GetStructLayoutResource() {
            return new LightStructBuffer(Type, Position, Direction, Color, Intensity);
        }
    }

    public class LightsSystem : BaseEntitySystem, IGraphicSystem {
        protected override void Executing(SceneSnapshot snapshot) {
            IEntityManager emanager = snapshot.ContextState.GetEntityManager();
            var camera = snapshot.Camera;

            try {
                foreach (var entity in emanager.GetEntities()) {
                    if (!entity.Has<ILightComponent>()) {
                        continue;
                    }
                    var light = entity.GetComponent<ILightComponent>();
                    var color = entity.GetComponent<ColorComponent>();

                    snapshot.UpdateLight(light.Index, new LightState {
                        Intensity = light.Intensity,
                        Position = light.Position,
                        Direction = light.Direction,
                        Color = color.Color,
                        Type = light.Type
                    });

                }
            } catch (Exception ex) {
                ex.ToString();
                throw ex;
            }
        }
    }
}
