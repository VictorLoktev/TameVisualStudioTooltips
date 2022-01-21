using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace TameVisualStudioTooltips3
{
    // Clean up
    // https://github.com/MicrosoftDocs/visualstudio-docs/blob/main/docs/extensibility/creating-an-extension-with-a-menu-command.md#clean-up-the-experimental-environment

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class TameQuickInfoCommand1
    {
        /// <summary>
        /// Command ID for tooltips displayes when CTRL & SHIFT are pressed.
        /// </summary>
        public const int CommandIdCtrlShift = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid( "c11cad95-224a-4881-b564-4bf95ea3fdda" );

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="TameQuickInfoCommand1"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private TameQuickInfoCommand1( AsyncPackage package, OleMenuCommandService commandService )
        {
            this.package = package ?? throw new ArgumentNullException( nameof( package ) );
            commandService = commandService ?? throw new ArgumentNullException( nameof( commandService ) );

            File.AppendAllText( @"d:\temp\dump", "TameQuickInfoCommand1: " + DateTime.Now.ToString() + "\r\n" );

            var menuCommandID = new CommandID( CommandSet, CommandIdCtrlShift );
            var menuItem = new MenuCommand( this.ExecuteCtrlShift, menuCommandID );
            commandService.AddCommand( menuItem );
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static TameQuickInfoCommand1 Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync( AsyncPackage package )
        {
            // Switch to the main thread - the call to AddCommand in Command1's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync( package.DisposalToken );

            OleMenuCommandService commandService = await package.GetServiceAsync( typeof( IMenuCommandService ) ) as OleMenuCommandService;
            Instance = new TameQuickInfoCommand1( package, commandService );
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteCtrlShift( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TameQuickInfoOptions.Instance.ShowTooltips = 1;
            TameQuickInfoOptions.Instance.Save();

            string message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & SHIFT keys are pressed";
            string title = "TameQuickInfo On/Off";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST );
        }
    }
}
