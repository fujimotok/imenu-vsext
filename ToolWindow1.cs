using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace imenu_vsext
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("a660f1e1-3bca-44ef-a15e-e84899ad2a74")]
    public class ToolWindow1 : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1"/> class.
        /// </summary>
        public ToolWindow1() : base(null)
        {
            this.Caption = "Imenu";

            ThreadHelper.ThrowIfNotOnUIThread();
            EnvDTE.DTE dte = ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ToolWindow1Control() { DataContext = new ToolWindow1VM(dte) };
        }
    }
}
