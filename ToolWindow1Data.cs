using Microsoft.VisualStudio.Extensibility.UI;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;

namespace Imenu
{
    /// <summary>
    /// ViewModel for the ToolWindow1Content remote user control.
    /// </summary>
    [DataContract]
    internal class ToolWindow1Data : NotifyPropertyChangedObject
    {
        public ToolWindow1Data()
        {
            this.JumpCommand = new AsyncCommand(this.JumpExecuteAsync);
            this._timer = new Timer((x) => { this.FilterItems(); }, null, 500, Timeout.Infinite);

            this._dataSouce.Add(new ImenuItem() { Name = "hello", Line = 1 });
            this._dataSouce.Add(new ImenuItem() { Name = "world", Line = 2 });
            this._dataSouce.Add(new ImenuItem() { Name = "hello world", Line = 3 });

            this.ImenuItems = this._dataSouce;
        }

        private Timer _timer;

        private string _text = string.Empty;
        [DataMember]
        public string Text
        {
            get => _text;
            set
            {
                SetProperty(ref this._text, value);
                this._timer.Change(300, Timeout.Infinite);
            }
        }

        private ObservableCollection<ImenuItem> _dataSouce = new ObservableCollection<ImenuItem>();

        private ImenuItem? _selectedItem = null;
        [DataMember]
        public ImenuItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref this._selectedItem, value);
        }

        private ObservableCollection<ImenuItem> _imenuItems = new ObservableCollection<ImenuItem>();
        [DataMember]
        public ObservableCollection<ImenuItem> ImenuItems
        {
            get => _imenuItems;
            set => SetProperty(ref this._imenuItems, value);
        }

        [DataMember]
        public AsyncCommand JumpCommand { get; private set; }

        private Task JumpExecuteAsync(object? parameter, CancellationToken token)
        {
            MessageBox.Show($"hello! {SelectedItem?.Line ?? 0}");

            return Task.CompletedTask;
        }

        private void FilterItems()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.ImenuItems = this._dataSouce;
                return;
            }

            var words = this.Text.Split(' ');

            // 検索文字列がすべて含まれるアイテムをフィルタリング
            var filteredItems = this._dataSouce.Where(item => words.All(word => item.Name.Contains(word)));

            this.ImenuItems = new ObservableCollection<ImenuItem>(filteredItems.ToList());
        }
    }
}
