using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.Shell;
using System.Runtime.Serialization;

namespace Imenu
{
    /// <summary>
    /// ViewModel for the ToolWindow1Content remote user control.
    /// </summary>
    [DataContract]
    internal class ToolWindow1Data : NotifyPropertyChangedObject
    {
        public ToolWindow1Data(EnvDTE.DTE? dte)
        {
            this._dte = dte;
            this.JumpCommand = new AsyncCommand(this.JumpExecuteAsync);
            this._timer = new Timer((x) => { this.FilterItems(); }, null, 500, Timeout.Infinite);

            this._dataSouce.Add(new ImenuItem() { Name = "hello", Line = 1 });
            this._dataSouce.Add(new ImenuItem() { Name = "world", Line = 2 });
            this._dataSouce.Add(new ImenuItem() { Name = "hello world", Line = 3 });

            this.ImenuItems = this._dataSouce;
        }

        private EnvDTE.DTE? _dte;
        private Timer _timer;

        private string _text = string.Empty;
        [DataMember]
        public string Text
        {
            get => _text;
            set
            {
                SetProperty(ref this._text, value);
                this._timer.Change(500, Timeout.Infinite);
            }
        }

        private List<ImenuItem> _dataSouce = new List<ImenuItem>();

        private ImenuItem? _selectedItem = null;
        [DataMember]
        public ImenuItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref this._selectedItem, value);
        }

        private List<ImenuItem> _imenuItems = new List<ImenuItem>();
        [DataMember]
        public List<ImenuItem> ImenuItems
        {
            get => _imenuItems;
            set => SetProperty(ref this._imenuItems, value);
        }

        [DataMember]
        public AsyncCommand JumpCommand { get; private set; }

        private async Task JumpExecuteAsync(object? parameter, CancellationToken token)
        {
            var line = SelectedItem?.Line ?? 0;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (this._dte == null)
            {
                return;
            }

            EnvDTE.Document? document = this._dte.ActiveDocument;
            EnvDTE.TextSelection? textSelection = (EnvDTE.TextSelection)document?.Selection;
            textSelection?.GotoLine(line, true);
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

            this.ImenuItems = filteredItems.ToList();
        }
    }
}
