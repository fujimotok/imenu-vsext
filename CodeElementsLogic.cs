using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;

namespace imenu_vsext
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
    }
}
