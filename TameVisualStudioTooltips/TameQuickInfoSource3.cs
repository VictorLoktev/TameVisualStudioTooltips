using System.Threading;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualStudio.Text;

namespace TameVisualStudioTooltips3
{
    internal class TameQuickInfoSource3 : IAsyncQuickInfoSource
    {
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
            var options = await TameQuickInfoOptions.GetLiveInstanceAsync();

            if( options.ShowTooltips > 0 )
            {
                // now on the UI thread
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Keyboard kbd = new Keyboard();
                switch( options.ShowTooltips )
                {
                    case 1: // Ctrl & Shift
                        if( !( kbd.CtrlKeyDown && kbd.ShiftKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case 2: // Ctrl & Shift & Alt
                        if( !( kbd.CtrlKeyDown && kbd.ShiftKeyDown && kbd.AltKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case 3: // Ctrl & Alt
                        if( !( kbd.CtrlKeyDown && kbd.AltKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case 4: // Ctrl
                        if( !( kbd.CtrlKeyDown ) )
                            await session.DismissAsync();
                        break;

                    case 5: // Shift
                        if( !( kbd.ShiftKeyDown) )
                            await session.DismissAsync();
                        break;

                    case 6: // Alt
                        if( !( kbd.AltKeyDown ) )
                            await session.DismissAsync();
                        break;
                }
            }

            return await Task.FromResult<QuickInfoItem>( null );
        }
    }
}
