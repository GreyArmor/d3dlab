using D3DLab.Std.Engine.Core;

namespace D3DLab {
    public sealed class GenneralContextState : BaseContextState {
        public GenneralContextState(ContextStateProcessor processor, EngineNotificator notificator) : base(processor, new ManagerContainer(notificator)) {
        }
    }


}
