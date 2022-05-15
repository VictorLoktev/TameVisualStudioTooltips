using System.Threading;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualStudio.Text;
using EnvDTE;

namespace TameVisualStudioTooltips3
{
    internal class TameQuickInfoSource3 : IAsyncQuickInfoSource
    {
        private static EnvDTE.DTE dte = null;

        public void Dispose()
        {
        }

        private ITextBuffer _textBuffer;

        public TameQuickInfoSource3( ITextBuffer textBuffer )
        {
            _textBuffer = textBuffer;
        }

        // This is called on a background thread.
        public async Task<QuickInfoItem> GetQuickInfoItemAsync( IAsyncQuickInfoSession session, CancellationToken cancellationToken )
        {
            // now on the UI thread
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if( dte == null )
                dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService( typeof( Microsoft.VisualStudio.Shell.Interop.SDTE ) ) as EnvDTE.DTE;

            // Unable to get DTE
            if( dte == null )
                return await Task.FromResult<QuickInfoItem>( null );

            dbgDebugMode vsState = dte.Debugger.CurrentMode;

            // No customization of QuickInfo (tooltip) in Debugger Run mode
            if( vsState == dbgDebugMode.dbgRunMode )
                return await Task.FromResult<QuickInfoItem>( null );

            // Customization of QuickInfo (tooltip) in Code Editing mode

            var options = await TameQuickInfoOptions.GetLiveInstanceAsync();

            if( vsState == dbgDebugMode.dbgDesignMode && options.ShowTooltipsEdit > 0 )
            {
                Keyboard kbd = new Keyboard();
                switch( options.ShowTooltipsEdit )
                {
                    case TameQuickInfoCommand.CommandIdCtrlShiftEdit: // Ctrl & Shift
                        if( !( kbd.CtrlKeyDown && kbd.ShiftKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case TameQuickInfoCommand.CommandIdCtrlAltShiftEdit: // Ctrl & Shift & Alt
                        if( !( kbd.CtrlKeyDown && kbd.ShiftKeyDown && kbd.AltKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case TameQuickInfoCommand.CommandIdCtrlAltEdit: // Ctrl & Alt
                        if( !( kbd.CtrlKeyDown && kbd.AltKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case TameQuickInfoCommand.CommandIdCtrlEdit: // Ctrl
                        if( !( kbd.CtrlKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case TameQuickInfoCommand.CommandIdShiftEdit: // Shift
                        if( !( kbd.ShiftKeyDown) )
                            await session.DismissAsync();
                        break;

                    case TameQuickInfoCommand.CommandIdAltEdit: // Alt
                        if( !( kbd.AltKeyDown ) )
                            await session.DismissAsync();
                        break;
                }
            }
            if( vsState == dbgDebugMode.dbgBreakMode && options.ShowTooltipsDebug > 0 )
            {
                Keyboard kbd = new Keyboard();
                switch( options.ShowTooltipsDebug )
                {
                case TameQuickInfoCommand.CommandIdCtrlShiftDebug: // Ctrl & Shift
                    if( !( kbd.CtrlKeyDown && kbd.ShiftKeyDown ) )
                        await session.DismissAsync();
                    break;

                case TameQuickInfoCommand.CommandIdCtrlAltShiftDebug: // Ctrl & Shift & Alt
                    if( !( kbd.CtrlKeyDown && kbd.ShiftKeyDown && kbd.AltKeyDown ) )
                        await session.DismissAsync();
                    break;

                case TameQuickInfoCommand.CommandIdCtrlAltDebug: // Ctrl & Alt
                    if( !( kbd.CtrlKeyDown && kbd.AltKeyDown ) )
                        await session.DismissAsync();
                    break;

                case TameQuickInfoCommand.CommandIdCtrlDebug: // Ctrl
                    if( !( kbd.CtrlKeyDown ) )
                        await session.DismissAsync();
                    break;

                case TameQuickInfoCommand.CommandIdShiftDebug: // Shift
                    if( !( kbd.ShiftKeyDown ) )
                        await session.DismissAsync();
                    break;

                case TameQuickInfoCommand.CommandIdAltDebug: // Alt
                    if( !( kbd.AltKeyDown ) )
                        await session.DismissAsync();
                    break;
                }
            }

            return await Task.FromResult<QuickInfoItem>( null );
        }
    }
}
