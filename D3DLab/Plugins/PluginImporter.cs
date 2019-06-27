using D3DLab.Plugin.Contracts.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

namespace D3DLab.Plugins {
    public class PluginImporter {
        public IEnumerable<IFileParserPlugin> ParserPlugins => operations.Select(x=>x.Value);
        
        [ImportMany(typeof(IFileParserPlugin))]
        IEnumerable<Lazy<IFileParserPlugin>> operations;

        const string folder = "plugins";
        readonly string directory;
        CompositionContainer container;

        public PluginImporter() {
            var here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            directory = Path.Combine(here, folder);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }

        public void Import() {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(directory));
            container = new CompositionContainer(catalog);
            try {
                container.ComposeParts(this);
            } catch (CompositionException ex) {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
                throw ex;
            }

        }
    }
}
