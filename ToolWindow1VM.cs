using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace imenu_vsext
{
    /// <summary>
    /// ViewModel for the ToolWindow1Content remote user control.
    /// </summary>
    public class ToolWindow1VM : INotifyPropertyChanged
    {
        public ToolWindow1VM(EnvDTE.DTE dte)
        {
            this._dte = dte;
            this._timer = new Timer((x) => { this.FilterItems(); }, null, 500, Timeout.Infinite);

            this.InitializeWindowEvents(this._dte);
        }

        private EnvDTE.DTE _dte;
        private WindowEvents _windowEvents;
        private Timer _timer;

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set
            {
                this._text = value;
                this.RaisePropertyChanged();
                this._timer.Change(500, Timeout.Infinite);
            }
        }

        private List<ImenuItem> _dataSouce = new List<ImenuItem>();

        private ImenuItem _selectedItem = null;
        public ImenuItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                this._selectedItem = value;
                this.RaisePropertyChanged();
            }
        }

        private ObservableCollection<ImenuItem> _imenuItems = new ObservableCollection<ImenuItem>();
        public ObservableCollection<ImenuItem> ImenuItems
        {
            get => _imenuItems;
            set
            {
                this._imenuItems = value;
                this.RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void InitializeWindowEvents(EnvDTE.DTE dte)
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

            if (GotFocus.Document == null)
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

                // フィルタ文字列をクリアするために個別にPropertyChangedを発火
                this._text = string.Empty;
                this.RaisePropertyChanged(nameof(this.Text));

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var model = document.ProjectItem.FileCodeModel;
                var elements = CodeElementsLogic.GetFlattenedCodeElements(model.CodeElements);
                this._dataSouce = elements.Select(element => new ImenuItem(element))
                                          .Where(item => item.Line != 0).ToList();
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

        public async Task JumpExecuteAsync()
        {
            var line = SelectedItem?.Line ?? 1;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (this._dte == null)
            {
                return;
            }

            EnvDTE.Document document = this._dte.ActiveDocument;
            EnvDTE.TextSelection textSelection = (EnvDTE.TextSelection)document.Selection;
            textSelection.GotoLine(line, true);
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
