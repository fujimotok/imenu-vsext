using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace imenu_vsext
{
    public class ImenuItem : INotifyPropertyChanged
    {
        public ImenuItem(CodeElement element)
        {
            Name = string.Empty;
            Line = 0;

            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                switch (element.Kind)
                {
                    case vsCMElement.vsCMElementFunction:
                        var function = (CodeFunction)element;
                        var paramNames = function.Parameters.OfType<CodeParameter>().Select(this.CodeParameterToString);
                        Name = $"Function: {function.Name} ({string.Join(", ", paramNames)})";
                        Line = function.StartPoint.Line;
                        break;
                    case vsCMElement.vsCMElementProperty:
                        var property = (CodeProperty)element;
                        Name = $"Property: {property.Name}";
                        Line = property.StartPoint.Line;
                        break;
                    case vsCMElement.vsCMElementVariable:
                        var variable = (CodeVariable)element;
                        Name = $"Variable: {variable.Name}";
                        Line = variable.StartPoint.Line;
                        break;
                    case vsCMElement.vsCMElementClass:
                        var codeClass = (CodeClass)element;
                        Name = $"Class: {codeClass.Name}";
                        Line = codeClass.StartPoint.Line;
                        break;
                    case vsCMElement.vsCMElementInterface:
                        var codeInterface = (CodeInterface)element;
                        Name = $"Interface: {codeInterface.Name}";
                        Line = codeInterface.StartPoint.Line;
                        break;
                    case vsCMElement.vsCMElementStruct:
                        var codeStruct = (CodeStruct)element;
                        Name = $"Struct: {codeStruct.Name}";
                        Line = codeStruct.StartPoint.Line;
                        break;
                    case vsCMElement.vsCMElementNamespace:
                        var codeNamespace = (CodeNamespace)element;
                        Name = $"Namespace: {codeNamespace.Name}";
                        Line = codeNamespace.StartPoint.Line;
                        break;
                    default:
                        break;
                }
            }
            catch
            {
            }
        }

        private string CodeParameterToString(CodeParameter parameter)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (parameter == null)
            {
                return string.Empty;
            }

            if (parameter.Type == null)
            {
                return parameter.Name;
            }
            else
            {
                var typeName = parameter.Type.AsString.Split('.').Last();
                return $"{typeName} {parameter.Name}";
            }
        }

        /// <summary>
        /// リスト表示名
        /// </summary>
        private string _name = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string Name
        {
            get => _name;
            set
            {
                this._name = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ジャンプ先情報
        /// </summary>
        public int Line { get; set; } = 0;
    }
}
