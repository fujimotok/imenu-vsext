using EnvDTE;
using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.ToolWindows;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using Microsoft.VisualStudio.Shell;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Imenu
{
    /// <summary>
    /// A sample tool window.
    /// </summary>
    [VisualStudioContribution]
    public class ToolWindow1 : ToolWindow
    {
        private ToolWindow1Content? content;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1" /> class.
        /// </summary>
        public ToolWindow1()
        {
            this.Title = "Imenu";
        }

        /// <inheritdoc />
        public override ToolWindowConfiguration ToolWindowConfiguration => new()
        {
            // Use this object initializer to set optional parameters for the tool window.
            Placement = ToolWindowPlacement.Floating,
        };

        /// <inheritdoc />
        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Use InitializeAsync for any one-time setup or initialization.

            await base.InitializeAsync(cancellationToken);
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE.DTE? dte = ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            this.content = new ToolWindow1Content(dte);
        }

        /// <inheritdoc />
        public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IRemoteUserControl>(this.content);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                content?.Dispose();

            base.Dispose(disposing);
        }
    }
}
