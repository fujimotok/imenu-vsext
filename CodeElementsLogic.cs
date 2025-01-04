using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Imenu
{
    internal class CodeElementsLogic
    {
        public static List<CodeElement> GetFlattenedCodeElements(CodeElements codeElements)
        {
            List<CodeElement> flattenedList = new List<CodeElement>();
            foreach (CodeElement element in codeElements)
            {
                FlattenCodeElements(element, flattenedList);
            }
            return flattenedList;
        }

        private static void FlattenCodeElements(CodeElement element, List<CodeElement> flattenedList)
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

        public static ImenuItem CodeElementToImenuItem(CodeElement element)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
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
                        return new ImenuItem() { Name = string.Empty, Line = 0 };
                }
            }
            catch
            {
                return new ImenuItem() { Name = string.Empty, Line = 0 }; ;
            }
        }

    }
}
