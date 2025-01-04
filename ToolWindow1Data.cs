using EnvDTE;
using Microsoft.VisualStudio.Extensibility.UI;
using Microsoft.VisualStudio.Shell;
using System.Collections.ObjectModel;
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

            this.InitializeWindowEvents(this._dte);
        }

        private EnvDTE.DTE? _dte;
        private WindowEvents? _windowEvents;
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

        private ObservableCollection<ImenuItem> _imenuItems = new ObservableCollection<ImenuItem>();
        [DataMember]
        public ObservableCollection<ImenuItem> ImenuItems
        {
            get => _imenuItems;
            set => SetProperty(ref this._imenuItems, value);
        }

        [DataMember]
        public AsyncCommand JumpCommand { get; private set; }

        private void InitializeWindowEvents(EnvDTE.DTE? dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (dte == null)
            {
                return;
            }

            // WindowEventsを保持しておかないとGCで解放されてしまうため、メンバー変数に保持
            this._windowEvents = dte.Events.WindowEvents;
            this._windowEvents.WindowActivated += OnWindowActivated;
        }

        private async void OnWindowActivated(Window GotFocus, Window LostFocus)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (GotFocus?.Document == null)
            {
                return;
            }

            await this.LoadImenuItemAsync(GotFocus.Document);
        }

        private async Task LoadImenuItemAsync(EnvDTE.Document document)
        {
            try
            {
                this._dataSouce.Clear();

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var model = document.ProjectItem.FileCodeModel;
                var elements = CodeElementsLogic.GetFlattenedCodeElements(model.CodeElements);

                foreach (CodeElement element in elements)
                {
                    var item = CodeElementsLogic.CodeElementToImenuItem(element);

                    if (item.Line != 0)
                    {
                        this._dataSouce.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e}");
            }
            finally
            {
                this.ImenuItems = new ObservableCollection<ImenuItem>(this._dataSouce);
            }
        }

        private async Task JumpExecuteAsync(object? parameter, CancellationToken token)
        {
            var line = SelectedItem?.Line ?? 1;

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
                this.ImenuItems = new ObservableCollection<ImenuItem>(this._dataSouce);
                return;
            }

            var words = this.Text.Split(' ');

            // 検索文字列がすべて含まれるアイテムをフィルタリング
            var filteredItems = this._dataSouce.Where(item => words.All(word => item.Name.ToLower().Contains(word.ToLower())));

            this.ImenuItems = new ObservableCollection<ImenuItem>(filteredItems.ToList());
        }
    }
}
