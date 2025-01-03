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

            this.InitializeDocumentEvents(this._dte);
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

        private ObservableCollection<ImenuItem> _imenuItems = new ObservableCollection<ImenuItem>();
        [DataMember]
        public ObservableCollection<ImenuItem> ImenuItems
        {
            get => _imenuItems;
            set => SetProperty(ref this._imenuItems, value);
        }

        [DataMember]
        public AsyncCommand JumpCommand { get; private set; }

        private void InitializeDocumentEvents(EnvDTE.DTE? dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (dte == null)
            {
                return;
            }

            dte.Events.DocumentEvents.DocumentOpened += this.OnDocumentChanged;
            dte.Events.DocumentEvents.DocumentSaved += this.OnDocumentChanged;
        }

        private async void OnDocumentChanged(Document document)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                this._dataSouce.Clear();

                var model = document.ProjectItem.FileCodeModel;
                var elements = this.GetFlattenedCodeElements(model.CodeElements);

                foreach (CodeElement element in elements)
                {
                    var item = this.CodeElementToImenuItem(element);

                    if (item != null)
                    {
                        this._dataSouce.Add(item);
                    }
                }

                this.ImenuItems = new ObservableCollection<ImenuItem>(this._dataSouce);
            }
            catch
            {

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

        private List<CodeElement> GetFlattenedCodeElements(CodeElements codeElements)
        {
            List<CodeElement> flattenedList = new List<CodeElement>();
            foreach (CodeElement element in codeElements)
            {
                FlattenCodeElements(element, flattenedList);
            }
            return flattenedList;
        }

        private void FlattenCodeElements(CodeElement element, List<CodeElement> flattenedList)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            flattenedList.Add(element);

            // 再帰的にメンバーを取得
            if (element is CodeNamespace)
            {
                CodeNamespace codeNamespace = (CodeNamespace)element;
                foreach (CodeElement child in codeNamespace.Members)
                {
                    FlattenCodeElements(child, flattenedList);
                }
            }
            else if (element is CodeClass)
            {
                CodeClass codeClass = (CodeClass)element;
                foreach (CodeElement child in codeClass.Members)
                {
                    FlattenCodeElements(child, flattenedList);
                }
            }
            else if (element is CodeStruct)
            {
                CodeStruct codeStruct = (CodeStruct)element;
                foreach (CodeElement child in codeStruct.Members)
                {
                    FlattenCodeElements(child, flattenedList);
                }
            }
            else if (element is CodeInterface)
            {
                CodeInterface codeInterface = (CodeInterface)element;
                foreach (CodeElement child in codeInterface.Members)
                {
                    FlattenCodeElements(child, flattenedList);
                }
            }
            else if (element is CodeFunction)
            {
                CodeFunction codeFunction = (CodeFunction)element;
                foreach (CodeElement child in codeFunction.Parameters)
                {
                    flattenedList.Add(child);
                }
            }
        }

        private ImenuItem CodeElementToImenuItem(CodeElement element)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            switch (element.Kind)
            {
                case vsCMElement.vsCMElementFunction:
                    var function = (CodeFunction)element;
                    return new ImenuItem() { Name = function.Name, Line = function.StartPoint.Line };
                case vsCMElement.vsCMElementProperty:
                    var property = (CodeProperty)element;
                    return new ImenuItem() { Name = property.Name, Line = property.StartPoint.Line };
                case vsCMElement.vsCMElementVariable:
                    var variable = (CodeVariable)element;
                    return new ImenuItem() { Name = variable.Name, Line = variable.StartPoint.Line };
                case vsCMElement.vsCMElementClass:
                    var codeClass = (CodeClass)element;
                    return new ImenuItem() { Name = codeClass.Name, Line = codeClass.StartPoint.Line };
                case vsCMElement.vsCMElementStruct:
                    var codeStruct = (CodeStruct)element;
                    return new ImenuItem() { Name = codeStruct.Name, Line = codeStruct.StartPoint.Line };
                case vsCMElement.vsCMElementInterface:
                    var codeInterface = (CodeInterface)element;
                    return new ImenuItem() { Name = codeInterface.Name, Line = codeInterface.StartPoint.Line };
                case vsCMElement.vsCMElementNamespace:
                    var codeNamespace = (CodeNamespace)element;
                    return new ImenuItem() { Name = codeNamespace.Name, Line = codeNamespace.StartPoint.Line };
                default:
                    return null;
            }
        }

    }
}
