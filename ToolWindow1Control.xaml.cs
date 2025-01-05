using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace imenu_vsext
{
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
        }

        private async void list_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var vm = this.DataContext as ToolWindow1VM;
            await vm?.JumpExecuteAsync();
        }

        private async void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Action listUp = () =>
            {
                var previousIndex = this.list.SelectedIndex - 1;
                var minIndex = 0;
                this.list.SelectedIndex = previousIndex > minIndex ? previousIndex : minIndex;
            };

            Action listDown = () =>
            {
                var nextIndex = this.list.SelectedIndex + 1;
                var maxIndex = this.list.Items.Count - 1;
                this.list.SelectedIndex = nextIndex < maxIndex ? nextIndex : maxIndex;
            };

            switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    listUp();
                    break;
                case System.Windows.Input.Key.Down:
                    listDown();
                    break;
                case System.Windows.Input.Key.Enter:
                    var vm = this.DataContext as ToolWindow1VM;
                    await vm?.JumpExecuteAsync();
                    break;
            }
        }
    }
}
