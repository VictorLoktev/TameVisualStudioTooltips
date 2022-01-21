using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace TameVisualStudioTooltips3
{
    // https://docs.microsoft.com/en-us/visualstudio/extensibility/walkthrough-displaying-quickinfo-tooltips?view=vs-2022

    [Export( typeof( IAsyncQuickInfoSourceProvider ) )]
    [Name( "Tame Visual Studio Tooltips 3" )]
    //[Order( After = "default" )]
    [Order]
    [ContentType("any")]
    class TameQuickInfo : IAsyncQuickInfoSourceProvider
    {
        public IAsyncQuickInfoSource TryCreateQuickInfoSource( ITextBuffer textBuffer )
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty( () => new TameQuickInfoSource3( textBuffer ) );
        }
    }
}
