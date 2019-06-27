using D3DLab.Debugger.Windows;
using D3DLab.Std.Engine.Core.Ext;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace D3DLab.Debugger.IDE {

    public partial class IntellisensePopup {
        public static IntellisensePopupPresenter Build(UIElement parent, TextPointer positionPointer, TextPoiterChanged target) {
            var view = new IntellisensePopup {
                PlacementTarget = parent,
                PlacementRectangle = positionPointer.GetCharacterRect(LogicalDirection.Forward)
            };
            view.presenter.PositionPointer = positionPointer;
            view.presenter.TargetVariable = target;
            return view.presenter;
        }

        readonly IntellisensePopupPresenter presenter;
        public IntellisensePopup() {
            InitializeComponent();
            this.StaysOpen = false;
            this.KeyDown += IntellisensePopup_KeyDown;
            presenter = new IntellisensePopupPresenter(this);
            Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            DataContext = presenter;
        }

        private void IntellisensePopup_KeyDown(object sender, KeyEventArgs e) {
            if(e.Key == Key.Escape) {
                presenter.Close();
            }
        }

        void OnListBoxItemMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            OnItemSelected(sender);
            e.Handled = true;
        }
        void OnItemSelected(object sender) {
            var item = (ListBoxItem)sender;
            presenter.OnMouseLeftButtonDown(item.DataContext.ToString());
           
        }

        void OnListBoxItemKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == System.Windows.Input.Key.Enter) {
                OnItemSelected(sender);
                e.Handled = true;
            }
        }
    }

    public class IntellisenseItem {

    }

    public class IntellisensePopupPresenter {
        public IntellisenseTypes Type { get; set; }
        public ShaderTextBox ShaderTextBox { get; set; }
        public TextPointer PositionPointer { get; set; }
        public TextPoiterChanged TargetVariable { get; set; }

        public ObservableCollection<string> Items { get; set; }
        public string VariableName { get; set; }

        readonly IntellisensePopup view;
        //readonly ObservableCollection<string> items;
        int offset;

        public IntellisensePopupPresenter(IntellisensePopup view) {
            this.view = view;
            Items = new ObservableCollection<string>();
            //items.Add("ToString()");
            //items.Add("Equals()");
            //items.Add("GetHasCode()");
            //Items = CollectionViewSource.GetDefaultView(items);
            //Items.MoveCurrentToPosition(-1);
            //Items.CurrentChanged += OnCurrentChanged;
        }

        public void OnMouseLeftButtonDown(string item) {
            switch (Type) {
                case IntellisenseTypes.AutoComplete:
                    item = item.Remove(0, VariableName.Length);
                    break;
            }
            PositionPointer.InsertTextInRun(item);
            offset = item.Length;
            Close();
        }
        
        public void Show() {
            view.IsOpen = true;
            Application.Current.Dispatcher.InvokeAsync(() => { view.ListBoxEle.Focus(); });
        }

        public void AddRange(IEnumerable<string> result) {
            result.ForEach(x => Items.Add(x));
        }

        public void Close() {
            view.IsOpen = false;
            Keyboard.Focus(ShaderTextBox);
            ShaderTextBox.CaretPosition = PositionPointer.GetPositionAtOffset(offset);
        }
    }
}
