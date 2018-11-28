﻿using D3DLab.Std.Engine.Core.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace D3DLab.Std.Engine.Core.Input {
    public class InputSnapshot {
        readonly ReaderWriterLockSlim loker;
        //private readonly object _loker;
        private Dictionary<Type, IInputCommand> cache;
        public InputSnapshot() {
            //_loker = new object();
            loker = new ReaderWriterLockSlim();//LockRecursionPolicy.SupportsRecursion
            cache = new Dictionary<Type, IInputCommand>();
        }

        public List<IInputCommand> Events {
            get {
                List<IInputCommand> values = null;
                using (new ReadLockSlim(loker)) {
                    values = cache.Values.ToList();
                }
                return values;
            }
        }
        public void AddEvent<TCommand>(TCommand ev) where TCommand : IInputCommand {
            using (new UpgradeableReadLockSlim(loker)) {
                var type = ev.GetType();
                if (cache.ContainsKey(type)) {
                    cache[type] = ev;
                } else {
                    using (new WriteLockSlim(loker)) {
                        cache.Add(type, ev);
                    }
                }
            }
        }
        public void RemoveEvent<TCommand>(TCommand ev) where TCommand : IInputCommand {
            using (new WriteLockSlim(loker)) {
                var type = ev.GetType();
                if (cache.ContainsKey(type)) {
                    cache.Remove(type);
                }
            }
            // Events.Remove(ev);
        }

        public InputSnapshot CloneAndClear() {
            Dictionary<Type, IInputCommand> temp;
            using (new WriteLockSlim(loker)) {
                temp = cache;
                cache = new Dictionary<Type, IInputCommand>();
            }
            var cloned = new InputSnapshot();
            foreach (var cmd in temp) {
                cloned.cache.Add(cmd.Key, cmd.Value);
            }
            //Console.WriteLine($"CloneAndClear {cloned.cache.Count}");
            return cloned;
        }

        internal void Dispose() {
            using (new WriteLockSlim(loker)) {
                cache.Clear();
            }
            loker.Dispose();
        }
    }
}
