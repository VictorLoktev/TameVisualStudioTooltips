using System.ComponentModel;

namespace TameVisualStudioTooltips3
{
    internal class TameQuickInfoOptions : TameQuickInfoBaseOptions<TameQuickInfoOptions>
    {
        [Category( "General" )]
        [DisplayName( "Show Tooltips" )]
        [Description( "Specifies whether to show editor tooltips (QuickInfo) when mouse hover " +
            "or when CTRL and SHIFT keys are pressed with mouse hover. " +
            "While Editing mode." )]
        [DefaultValue( TameQuickInfoCommand.CommandIdAlwaysEdit )]
        public int ShowTooltipsEdit { get; set; }

        [Category( "General" )]
        [DisplayName( "Show Tooltips" )]
        [Description( "Specifies whether to show editor tooltips (QuickInfo) when mouse hover " +
            "or when CTRL and SHIFT keys are pressed with mouse hover. " +
            "While Debugger break mode." )]
        [DefaultValue( TameQuickInfoCommand.CommandIdAlwaysDebug )]
        public int ShowTooltipsDebug { get; set; }
    }
}
