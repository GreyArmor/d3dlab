

using System;

namespace D3DLab.Std.Engine.Core {
    internal interface IComponentSystemIncrementId {
        int ID { set; }
    }
    public interface IGraphicSystem {
        int ID { get; }
        TimeSpan ExecutionTime { get; }
        void Execute(SceneSnapshot snapshot);        
    }
}
