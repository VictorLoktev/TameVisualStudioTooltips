using System.ComponentModel;

namespace TameVisualStudioTooltips3
{
    internal class TameQuickInfoOptions : TameQuickInfoBaseOptions<TameQuickInfoOptions>
    {
        [Category( "General" )]
        [DisplayName( "Show Tooltips" )]
        [Description( "Specifies whether to show editor tooltips (QuickInfo) when mouse hover or when CTRL and SHIFT keys are pressed with mouse hover." )]
        [DefaultValue( 0 )]
        public int ShowTooltips { get; set; }
    }
}
