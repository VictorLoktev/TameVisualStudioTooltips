using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    internal sealed class TameQuickInfoCommand
    {
        #region While in Editing mode

        /// <summary>
        /// Command ID for always show tooltips.
        /// </summary>
        public const int CommandIdAlwaysEdit = 0x1100;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & SHIFT are pressed.
        /// </summary>
        public const int CommandIdCtrlShiftEdit = 0x1101;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & ALT & SHIFT are pressed.
        /// </summary>
        public const int CommandIdCtrlAltShiftEdit = 0x1102;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & ALT are pressed.
        /// </summary>
        public const int CommandIdCtrlAltEdit = 0x1103;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL is pressed.
        /// </summary>
        public const int CommandIdCtrlEdit = 0x1104;

        /// <summary>
        /// Command ID for tooltips displayes when Shift is pressed.
        /// </summary>
        public const int CommandIdShiftEdit = 0x1105;

        /// <summary>
        /// Command ID for tooltips displayes when ALT is pressed.
        /// </summary>
        public const int CommandIdAltEdit = 0x1106;

        private readonly int[] EditCommands =
            {
                CommandIdAlwaysEdit, CommandIdCtrlShiftEdit, CommandIdCtrlAltShiftEdit,
                CommandIdCtrlAltEdit, CommandIdCtrlEdit, CommandIdShiftEdit, CommandIdAltEdit
            };

        #endregion

        #region While in Debugger break mode

        /// <summary>
        /// Command ID for always show tooltips.
        /// </summary>
        public const int CommandIdAlwaysDebug = 0x2100;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & SHIFT are pressed.
        /// </summary>
        public const int CommandIdCtrlShiftDebug = 0x2101;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & ALT & SHIFT are pressed.
        /// </summary>
        public const int CommandIdCtrlAltShiftDebug = 0x2102;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & ALT are pressed.
        /// </summary>
        public const int CommandIdCtrlAltDebug = 0x2103;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL is pressed.
        /// </summary>
        public const int CommandIdCtrlDebug = 0x2104;

        /// <summary>
        /// Command ID for tooltips displayes when Shift is pressed.
        /// </summary>
        public const int CommandIdShiftDebug = 0x2105;

        /// <summary>
        /// Command ID for tooltips displayes when ALT is pressed.
        /// </summary>
        public const int CommandIdAltDebug = 0x2106;

        private readonly int[] DebugCommands =
            {
                CommandIdAlwaysDebug, CommandIdCtrlShiftDebug, CommandIdCtrlAltShiftDebug,
                CommandIdCtrlAltDebug, CommandIdCtrlDebug, CommandIdShiftDebug, CommandIdAltDebug
            };

        #endregion


        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid( "c11cad95-224a-4881-b564-4bf95ea3fdda" );

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private Dictionary<int, MenuCommand> Commands = new Dictionary<int, MenuCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TameQuickInfoCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private TameQuickInfoCommand( AsyncPackage package, OleMenuCommandService commandService )
        {
            this.package = package ?? throw new ArgumentNullException( nameof( package ) );
            if( commandService == null )
                throw new ArgumentNullException( nameof( commandService ) );

#pragma warning disable VSTHRD102 // Implement internal logic asynchronously
            var options = ThreadHelper.JoinableTaskFactory.Run( TameQuickInfoOptions.GetLiveInstanceAsync );
#pragma warning restore VSTHRD102 // Implement internal logic asynchronously
            int debug = options.ShowTooltipsDebug;
            int edit = options.ShowTooltipsEdit;

            foreach( var id in EditCommands )
            {
                var menuCommandID = new CommandID( CommandSet, id );
                var menuItem = new MenuCommand( this.ExecuteCommand, menuCommandID );
                menuItem.Checked = id == edit;
                commandService.AddCommand( menuItem );
                Commands.Add( id, menuItem );
            }

            foreach( var id in DebugCommands )
            {
                var menuCommandID = new CommandID( CommandSet, id );
                var menuItem = new MenuCommand( this.ExecuteCommand, menuCommandID );
                menuItem.Checked = id == debug;
                commandService.AddCommand( menuItem );
                Commands.Add( id, menuItem );
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static TameQuickInfoCommand Instance
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
            Instance = new TameQuickInfoCommand( package, commandService );
        }

        private void UncheckEditComamnds()
        {
            foreach( var id in EditCommands )
            {
                if( Commands.TryGetValue( id, out var menuItem ) )
                {
                    menuItem.Checked = false;
                }
            }
        }

        private void UncheckDebugComamnds()
        {
            foreach( var id in DebugCommands )
            {
                if( Commands.TryGetValue( id, out var menuItem ) )
                {
                    menuItem.Checked = false;
                }
            }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteCommand( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string message;
            string title;

            int id = ( sender as MenuCommand ).CommandID.ID;

            switch( id )
            {
            case CommandIdAlwaysEdit:
                TameQuickInfoOptions.Instance.ShowTooltipsEdit = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover";
                title = "TameQuickInfo On/Off in Editing mode";
                UncheckEditComamnds();
                break;

            case CommandIdCtrlShiftEdit:
                TameQuickInfoOptions.Instance.ShowTooltipsEdit = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & SHIFT keys are pressed";
                title = "TameQuickInfo On/Off in Editing mode";
                UncheckEditComamnds();
                break;

            case CommandIdCtrlAltShiftEdit:
                TameQuickInfoOptions.Instance.ShowTooltipsEdit = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & SHIFT & ALT keys are pressed";
                title = "TameQuickInfo On/Off in Editing mode";
                UncheckEditComamnds();
                break;

            case CommandIdCtrlAltEdit:
                TameQuickInfoOptions.Instance.ShowTooltipsEdit = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & ALT keys are pressed";
                title = "TameQuickInfo On/Off in Editing mode";
                UncheckEditComamnds();
                break;

            case CommandIdCtrlEdit:
                TameQuickInfoOptions.Instance.ShowTooltipsEdit = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL key is pressed";
                title = "TameQuickInfo On/Off in Editing mode";
                UncheckEditComamnds();
                break;

            case CommandIdShiftEdit:
                TameQuickInfoOptions.Instance.ShowTooltipsEdit = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when SHIFT key is pressed";
                title = "TameQuickInfo On/Off in Editing mode";
                UncheckEditComamnds();
                break;

            case CommandIdAltEdit:
                TameQuickInfoOptions.Instance.ShowTooltipsEdit = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when ALT key is pressed";
                title = "TameQuickInfo On/Off in Editing mode";
                UncheckEditComamnds();
                break;


            case CommandIdAlwaysDebug:
                TameQuickInfoOptions.Instance.ShowTooltipsDebug = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover";
                title = "TameQuickInfo On/Off in Debugger break mode";
                UncheckDebugComamnds();
                break;

            case CommandIdCtrlShiftDebug:
                TameQuickInfoOptions.Instance.ShowTooltipsDebug = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & SHIFT keys are pressed";
                title = "TameQuickInfo On/Off in Debugger break mode";
                UncheckDebugComamnds();
                break;

            case CommandIdCtrlAltShiftDebug:
                TameQuickInfoOptions.Instance.ShowTooltipsDebug = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & SHIFT & ALT keys are pressed";
                title = "TameQuickInfo On/Off in Debugger break mode";
                UncheckDebugComamnds();
                break;

            case CommandIdCtrlAltDebug:
                TameQuickInfoOptions.Instance.ShowTooltipsDebug = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & ALT keys are pressed";
                title = "TameQuickInfo On/Off in Debugger break mode";
                UncheckDebugComamnds();
                break;

            case CommandIdCtrlDebug:
                TameQuickInfoOptions.Instance.ShowTooltipsDebug = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL key is pressed";
                title = "TameQuickInfo On/Off in Debugger break mode";
                UncheckDebugComamnds();
                break;

            case CommandIdShiftDebug:
                TameQuickInfoOptions.Instance.ShowTooltipsDebug = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when SHIFT key is pressed";
                title = "TameQuickInfo On/Off in Debugger break mode";
                UncheckDebugComamnds();
                break;

            case CommandIdAltDebug:
                TameQuickInfoOptions.Instance.ShowTooltipsDebug = id;
                message = "Tooltips (QuickInfo) will be displayed by mouse hover when ALT key is pressed";
                title = "TameQuickInfo On/Off in Debugger break mode";
                UncheckDebugComamnds();
                break;


            default:
                throw new NotSupportedException( "Unknown command ID" );
            }

            ( sender as MenuCommand ).Checked = true;

            TameQuickInfoOptions.Instance.Save();

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
