﻿using System;
using System.Collections.Generic;
using System.Text;

namespace D3DLab.Std.Engine.Core.Systems {
    public sealed class InputSystem : BaseComponentSystem, IComponentSystem {
        public void Execute(SceneSnapshot snapshot) {
            var s = snapshot.Snapshot;

            foreach (var en in snapshot.ContextState.GetEntityManager().GetEntities()) {
                foreach (var cmd in s.Events) {
                    if (cmd.Execute(en)) {
                        s.RemoveEvent(cmd);
                    }
                }
            }
        }
    }
}
