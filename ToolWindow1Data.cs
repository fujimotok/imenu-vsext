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

            this.ImenuItems.Add(new ImenuItem() { Name = "item1", Line = 1 });
            this.ImenuItems.Add(new ImenuItem() { Name = "item1", Line = 2 });
            this.ImenuItems.Add(new ImenuItem() { Name = "item1", Line = 3 });
            this.ImenuItems.Add(new ImenuItem() { Name = "item1", Line = 4 });
            this.ImenuItems.Add(new ImenuItem() { Name = "item1", Line = 5 });
        }

        private string _text = string.Empty;
        [DataMember]
        public string Text
        {
            get => _text;
            set => SetProperty(ref this._text, value);
        }

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
    }
}
