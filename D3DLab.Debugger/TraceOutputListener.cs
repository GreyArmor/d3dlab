﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace D3DLab.Debugger {
    public class TraceOutputListener : System.Diagnostics.TraceListener {
        readonly ObservableCollection<string> output;
        readonly Dispatcher dispatcher;
        const int maxlines = 100;
        public TraceOutputListener(ObservableCollection<string> consoleOutput, Dispatcher dispatcher) {
            this.output = consoleOutput;
            this.dispatcher = dispatcher;
        }

        public override void Write(string message) {

        }

        public override void WriteLine(string message) {
            dispatcher.InvokeAsync(() => {
                output.Insert(0, $"[{DateTime.Now.TimeOfDay}] {message.Trim()}");
                if (output.Count > maxlines) {
                    output.RemoveAt(maxlines);
                }
            });
        }
    }
}
