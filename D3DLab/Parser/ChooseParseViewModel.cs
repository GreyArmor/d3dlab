using D3DLab.Debugger;
using D3DLab.Plugin.Contracts.Parsers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace D3DLab.Parser {
    public class ImportingFile {
        public string Name => Path.GetFileName(path);

        public FileInfo FileInfo => new FileInfo(path);

        readonly string path;
        public ImportingFile(string f) {
            this.path = f;
        }
    }
    public class ParserTypeItem {
        readonly IFileParserPlugin parser;

        public ParserTypeItem(IFileParserPlugin i) {
            this.parser = i;
        }

        public string Name => parser.Name;
        public IFileParserPlugin Parser => parser;
    }
    public class ImportFileInfo {
        public readonly FileInfo File;
        public readonly IFileParserPlugin Parser;
        public readonly bool IsWatching;
        public ImportFileInfo(FileInfo file, IFileParserPlugin parser, bool watching) {
            File = file;
            Parser = parser;
            IsWatching = watching;
        }
    }

    public interface IFileLoader {
        void Load(ImportFileInfo info);
    }

    public class ChooseParseViewModel {
        class LoadCmd : BaseWPFCommand<Window> {
            readonly ChooseParseViewModel facade;

            public LoadCmd(ChooseParseViewModel chooseParseViewModel) {
                this.facade = chooseParseViewModel;
            }

            public override void Execute(Window win) {
                facade.LoadSelected();
                win.Close();
            }
        }

        

        public ICommand Load { get; }

        ObservableCollection<ImportingFile> importingFiles;
        ObservableCollection<ParserTypeItem> parserTypes;

        public ICollectionView ImportingFiles { get; }
        public ICollectionView ParserTypes { get; }

        IFileLoader loader;
        public ChooseParseViewModel() {
            importingFiles = new ObservableCollection<ImportingFile>();
            parserTypes = new ObservableCollection<ParserTypeItem>();

            ImportingFiles = CollectionViewSource.GetDefaultView(importingFiles);
            ParserTypes = CollectionViewSource.GetDefaultView(parserTypes);

            Load = new LoadCmd(this);
        }

        public void SetLoader(IFileLoader loader) {
            this.loader = loader;
        }
        private void LoadSelected() {
            var file =(ImportingFile) ImportingFiles.CurrentItem;
            var parser = (ParserTypeItem)ParserTypes.CurrentItem;
            App.Current.Dispatcher.InvokeAsync(() => {
                loader.Load(new ImportFileInfo(file.FileInfo, parser.Parser, true));
            });
        }

        public void AddFiles(string[] files) {
            foreach(var i in files) {
               importingFiles.Add(new ImportingFile(i));
            }
            ImportingFiles.MoveCurrentToFirst();
        }

        public void AddParsers(IEnumerable<IFileParserPlugin> parsers) {
            foreach (var i in parsers) {
                parserTypes.Add(new ParserTypeItem(i));
            }

            ParserTypes.MoveCurrentToFirst();
        }

    }
}
