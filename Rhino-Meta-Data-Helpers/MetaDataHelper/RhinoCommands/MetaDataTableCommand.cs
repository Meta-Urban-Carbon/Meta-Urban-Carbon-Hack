using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System.Runtime.InteropServices;

namespace MetaDataHelper.Commands
{
    [System.Runtime.InteropServices.Guid("FDB2E92F-D4F4-44C1-87CA-9B16CC99AA4C")]

    public class MetaDataTableCommand : Command
    {
        public MetaDataTableCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static MetaDataTableCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "MetaDataTableCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            System.Guid panelId = MetaDataTablePanelHost.PanelId;
            bool bVisible = Rhino.UI.Panels.IsPanelVisible(panelId);

            string prompt = (bVisible)
                ? "Table panel is visible. New value"
                : "Table panel is hidden. New value";

            Rhino.Input.Custom.GetOption go = new Rhino.Input.Custom.GetOption();
            int hide_index = go.AddOption("Hide");
            int show_index = go.AddOption("Show");
            int toggle_index = go.AddOption("Toggle");

            go.Get();
            if (go.CommandResult() != Rhino.Commands.Result.Success)
                return go.CommandResult();

            Rhino.Input.Custom.CommandLineOption option = go.Option();
            if (null == option)
                return Rhino.Commands.Result.Failure;

            int index = option.Index;

            if (index == hide_index)
            {
                if (bVisible)
                    Rhino.UI.Panels.ClosePanel(panelId);
            }
            else if (index == show_index)
            {
                if (!bVisible)
                    Rhino.UI.Panels.OpenPanel(panelId);
            }
            else if (index == toggle_index)
            {
                if (bVisible)
                    Rhino.UI.Panels.ClosePanel(panelId);
                else
                    Rhino.UI.Panels.OpenPanel(panelId);
            }
            return Result.Success;
        }
    }
}
