using System.Runtime.InteropServices;

namespace MetaDataHelper
{
    /// <summary>
    /// Rhino framework requires a System.Windows.Forms.IWin32Window derived object for a panel.
    /// </summary>
    [System.Runtime.InteropServices.Guid("30EBB8C9-7FCB-43D0-A83E-0A934BB59151")]

    public class MetaDataTablePanelHost : RhinoWindows.Controls.WpfElementHost
    {
        public MetaDataTablePanelHost(uint docSn)
            : base(new TablePanelUserControl(docSn),null)
        {
        }

        /// <summary>
        /// Returns the ID of this panel.
        /// </summary>
        public static System.Guid PanelId
        {
            get
            {
                return typeof(MetaDataTablePanelHost).GUID;
            }
        }
    }
}
