namespace MetaDataHelper
{
    /// <summary>
    /// Rhino framework requires a System.Windows.Forms.IWin32Window derived object for a panel.
    /// </summary>
    [System.Runtime.InteropServices.Guid("BDFBAF41-D939-419E-8603-2FC6AC5806A7")]
    public class ClassManagerPanelHost : RhinoWindows.Controls.WpfElementHost
    {
        public ClassManagerPanelHost()
            : base(new MetaDataHelperPanelUserControl(), new ViewModel())
        {
        }

        /// <summary>
        /// Returns the ID of this panel.
        /// </summary>
        public static System.Guid PanelId
        {
            get
            {
                return typeof(ClassManagerPanelHost).GUID;
            }
        }
    }
}
