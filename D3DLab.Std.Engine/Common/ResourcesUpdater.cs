using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.Utilities;

namespace D3DLab.Std.Engine.Common {
    public class ResourcesUpdater {

        public ResourceLayout Layout { get; private set; }
        public ResourceSet Set { get; private set; }

        DisposeCollectorResourceFactory factory;
        CommandList cmd;
        readonly ResourceLayoutDescription layoutDescription;

        public ResourcesUpdater(ResourceLayoutDescription layoutDescription) {
            this.layoutDescription = layoutDescription;
        }

        public void Update(DisposeCollectorResourceFactory factory, CommandList commands) {
            this.factory = factory;
            this.cmd = commands;
        }

        public void UpdateResourceLayout() {
            if (Layout == null) {
                Layout = factory.CreateResourceLayout(layoutDescription);
            }
        }
        public void UpdateResourceSet(ResourceSetDescription description) {
            if (Set != null) {
                Set.Dispose();
            }
            Set = factory.CreateResourceSet(description);
        }
    }
}
