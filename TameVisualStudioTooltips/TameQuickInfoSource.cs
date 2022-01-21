using System.Threading;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using System.IO;
using System;
using Microsoft.VisualBasic.Devices;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Thread = EnvDTE.Thread;

namespace TameVisualStudioTooltips3
{
    internal class TameQuickInfoSource : IAsyncQuickInfoSource
    {
        private DateTime? NextShow = null;
        //private bool Displayed = false;
        private bool IsDisabled = false;

        public TameQuickInfoSource()
        {
        }

        public void Dispose()
        {
        }

        // This is called on a background thread.
        public async Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var isKeyDown = false;

            await ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                // now on the UI thread
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Keyboard kbd = new Keyboard();
                isKeyDown = kbd.ShiftKeyDown && kbd.CtrlKeyDown;
                if( kbd.ShiftKeyDown && kbd.CtrlKeyDown && kbd.AltKeyDown )
                {
                    IsDisabled = !IsDisabled;
                    IVsStatusbar bar = Package.GetGlobalService( typeof( IVsStatusbar ) ) as IVsStatusbar;
                    bar.IsFrozen( out int pfFrozen );
                    if( pfFrozen != 0 )
                    {
                        bar.FreezeOutput( 0 );
                    }
                    bar.SetText( $"Tame Visual Studio Tooltips is {( IsDisabled ? "disabled" : "enabled" )}." );
                }
            } );

            // back on background thread
            if (!isKeyDown && !IsDisabled)
            {
                _ = session.DismissAsync();

                // Every 15 minutes remind about keys
                if ((NextShow ?? DateTime.Today) < DateTime.Now)
                {
                    _ = ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                    {
                        NextShow = DateTime.Now.AddMinutes(15);
                        // now on the UI thread
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        IVsStatusbar bar = Package.GetGlobalService(typeof(IVsStatusbar)) as IVsStatusbar;
                        bar.IsFrozen(out int pfFrozen);
                        if (pfFrozen == 0)
                        {
                            bar.SetText("Press CTRL & SHIFT both down to see tooltip (QuickInfo). CTRL & ALT & SHIFT to disabled.");
                        }
                        //Displayed = true;
                        //bar.IsFrozen(out int pfFrozen);
                        //if (pfFrozen != 0)
                        //{
                        //    bar.FreezeOutput(0);
                        //}
                        //bar.FreezeOutput(1);
                        //System.Threading.Thread.Sleep(5000);
                        //bar.FreezeOutput(0);
                        //bar.Clear();
                        //Displayed = false;
                    });
                }
            }
            //else
            //{
            //    if (Displayed)
            //    {
            //        _ = ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            //        {
            //            NextShow = DateTime.Now.AddSeconds(30);
            //            // now on the UI thread
            //            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            //            IVsStatusbar bar = Package.GetGlobalService(typeof(IVsStatusbar)) as IVsStatusbar;
            //            bar.IsFrozen(out int pfFrozen);
            //            if (pfFrozen != 0)
            //            {
            //                bar.FreezeOutput(0);
            //                bar.Clear();
            //                Displayed = false;
            //            }
            //        });
            //    }
            //}

            if( IsDisabled && ( NextShow ?? DateTime.Today ) < DateTime.Now )
            {
                _ = ThreadHelper.JoinableTaskFactory.RunAsync( async delegate
                {
                    NextShow = DateTime.Now.AddMinutes( 15 );
                    // now on the UI thread
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    IVsStatusbar bar = Package.GetGlobalService( typeof( IVsStatusbar ) ) as IVsStatusbar;
                    bar.IsFrozen( out int pfFrozen );
                    if( pfFrozen == 0 )
                    {
                        bar.SetText( "Tame Quick Info is disabled. Press CTRL & ALT & SHIFT for tooltip to enable." );
                    }
                    //Displayed = true;
                    //bar.IsFrozen(out int pfFrozen);
                    //if (pfFrozen != 0)
                    //{
                    //    bar.FreezeOutput(0);
                    //}
                    //bar.FreezeOutput(1);
                    //System.Threading.Thread.Sleep(5000);
                    //bar.FreezeOutput(0);
                    //bar.Clear();
                    //Displayed = false;
                } );
            }

            return await System.Threading.Tasks.Task.FromResult<QuickInfoItem>(null);
            //return null;
        }
    }
}
