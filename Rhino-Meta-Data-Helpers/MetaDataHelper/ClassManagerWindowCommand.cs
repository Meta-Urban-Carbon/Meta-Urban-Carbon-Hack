using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace MetaDataHelper
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
        public override string EnglishName => "UserStringClassManager";

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
