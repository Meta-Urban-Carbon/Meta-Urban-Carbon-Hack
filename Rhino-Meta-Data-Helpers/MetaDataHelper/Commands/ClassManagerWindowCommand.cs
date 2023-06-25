using Rhino;
using Rhino.Commands;

namespace MetaDataHelper.Commands
{
    internal class ClassManagerWindowCommand : Command
    {
        public ClassManagerWindowCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ClassManagerWindowCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "ClassManagerWindowCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Notify User
            RhinoApp.WriteLine("Launching UserString Class Manager", EnglishName);

            // Start Application
            var ClassManagerApplication = new ClassManagerWindowApp();
            ClassManagerApplication.Run();

            return Result.Success;
        }
    }
}
