using System;
using System.IO;

namespace MetaDataHelper
{
    internal class Settings
    {
        /// <summary>
        /// Default Plugin Working Folder
        /// @"McNeel\Rhinoceros\7.0\Plug-ins\MetaDataHelper\SavedTemplates"
        /// </summary>
        public static string DefaultTemplatePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            @"McNeel\Rhinoceros\7.0\Plug-ins\MetaDataHelper\SavedTemplates");
    }
}
