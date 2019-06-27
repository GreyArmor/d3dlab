using D3DLab.Debugger.Presentation.PropertiesEditor;
using D3DLab.Debugger.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3DLab.Debugger.Presentation {
    public abstract class OpenPropertiesEditorCommand<T> : BaseWPFCommand<T> {
        readonly IRenderUpdater updater;

        public OpenPropertiesEditorCommand(IRenderUpdater updater) {
            this.updater = updater;
        }

        public override void Execute(T parameter) {
            var win = new PropertiesEditorPopup(updater);
            win.ViewModel.Analyze(Convert(parameter));
            win.Show();
        }

        protected abstract IEditingProperties Convert(T item);
    }
}
