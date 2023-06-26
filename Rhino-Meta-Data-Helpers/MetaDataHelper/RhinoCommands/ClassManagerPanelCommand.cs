using Rhino;
using Rhino.Commands;

namespace MetaDataHelper
{
    [System.Runtime.InteropServices.Guid("0E4A91C4-D308-462D-B6E2-5F4420E211A6")]

    public class ClassManagerPanelCommand : Command
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public ClassManagerPanelCommand()
        {
        }

        /// <summary>
        /// The command name as it appears on the Rhino command line.
        /// </summary>
        public override string EnglishName => "ClassManagerPanelCommand";

        /// <summary>
        /// Called by Rhino when the user wants to run this command.
        /// </summary>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            System.Guid panelId = ClassManagerPanelHost.PanelId;
            bool bVisible = Rhino.UI.Panels.IsPanelVisible(panelId);

            string prompt = (bVisible)
                ? "Class Manager panel is visible. New value"
                : "Class Manager panel is hidden. New value";

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
