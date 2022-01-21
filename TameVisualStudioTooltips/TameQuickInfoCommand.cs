﻿using System;
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
    internal sealed class TameQuickInfoCommand
    {
        /// <summary>
        /// Command ID for always show tooltips.
        /// </summary>
        public const int CommandIdAlways = 0x0100;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & SHIFT are pressed.
        /// </summary>
        public const int CommandIdCtrlShift = 0x0101;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & ALT & SHIFT are pressed.
        /// </summary>
        public const int CommandIdCtrlAltShift = 0x0102;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL & ALT are pressed.
        /// </summary>
        public const int CommandIdCtrlAlt = 0x0103;

        /// <summary>
        /// Command ID for tooltips displayes when CTRL is pressed.
        /// </summary>
        public const int CommandIdCtrl = 0x0104;

        /// <summary>
        /// Command ID for tooltips displayes when Shift is pressed.
        /// </summary>
        public const int CommandIdShift = 0x0105;

        /// <summary>
        /// Command ID for tooltips displayes when ALT is pressed.
        /// </summary>
        public const int CommandIdAlt = 0x0106;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid( "c11cad95-224a-4881-b564-4bf95ea3fdda" );

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="TameQuickInfoCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private TameQuickInfoCommand( AsyncPackage package, OleMenuCommandService commandService )
        {
            this.package = package ?? throw new ArgumentNullException( nameof( package ) );
            commandService = commandService ?? throw new ArgumentNullException( nameof( commandService ) );

            {
                var menuCommandID = new CommandID( CommandSet, CommandIdAlways );
                var menuItem = new MenuCommand( this.ExecuteAllways, menuCommandID );
                commandService.AddCommand( menuItem );
            }
            {
                var menuCommandID = new CommandID( CommandSet, CommandIdCtrlShift );
                var menuItem = new MenuCommand( this.ExecuteCtrlShift, menuCommandID );
                commandService.AddCommand( menuItem );
            }
            {
                var menuCommandID = new CommandID( CommandSet, CommandIdCtrlAltShift );
                var menuItem = new MenuCommand( this.ExecuteCtrlAltShift, menuCommandID );
                commandService.AddCommand( menuItem );
            }
            {
                var menuCommandID = new CommandID( CommandSet, CommandIdCtrlAlt );
                var menuItem = new MenuCommand( this.ExecuteCtrlAlt, menuCommandID );
                commandService.AddCommand( menuItem );
            }
            {
                var menuCommandID = new CommandID( CommandSet, CommandIdCtrl );
                var menuItem = new MenuCommand( this.ExecuteCtrl, menuCommandID );
                commandService.AddCommand( menuItem );
            }
            {
                var menuCommandID = new CommandID( CommandSet, CommandIdShift );
                var menuItem = new MenuCommand( this.ExecuteShift, menuCommandID );
                commandService.AddCommand( menuItem );
            }
            {
                var menuCommandID = new CommandID( CommandSet, CommandIdAlt );
                var menuItem = new MenuCommand( this.ExecuteAlt, menuCommandID );
                commandService.AddCommand( menuItem );
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

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteAllways( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TameQuickInfoOptions.Instance.ShowTooltips = 0;
            TameQuickInfoOptions.Instance.Save();

            string message = "Tooltips (QuickInfo) will be displayed by mouse hover";
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

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteCtrlAltShift( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TameQuickInfoOptions.Instance.ShowTooltips = 2;
            TameQuickInfoOptions.Instance.Save();

            string message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & SHIFT & ALT keys are pressed";
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

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteCtrlAlt( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TameQuickInfoOptions.Instance.ShowTooltips = 3;
            TameQuickInfoOptions.Instance.Save();

            string message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL & ALT keys are pressed";
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

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteCtrl( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TameQuickInfoOptions.Instance.ShowTooltips = 4;
            TameQuickInfoOptions.Instance.Save();

            string message = "Tooltips (QuickInfo) will be displayed by mouse hover when CTRL key is pressed";
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

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteShift( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TameQuickInfoOptions.Instance.ShowTooltips = 5;
            TameQuickInfoOptions.Instance.Save();

            string message = "Tooltips (QuickInfo) will be displayed by mouse hover when SHIFT key is pressed";
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

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void ExecuteAlt( object sender, EventArgs e )
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TameQuickInfoOptions.Instance.ShowTooltips = 6;
            TameQuickInfoOptions.Instance.Save();

            string message = "Tooltips (QuickInfo) will be displayed by mouse hover when ALT key is pressed";
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
