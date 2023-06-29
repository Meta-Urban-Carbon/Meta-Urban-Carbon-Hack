using MetaDataHelper.UserStringClass;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;

namespace MetaDataHelper
{
    internal class SaveTemplateCommand: ICommand
    {

        private UserStringTemplate _currentTemplate;

        public bool CanExecute(object parameter)
        {
            return true;
            //throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            var CurrentTemplate = parameter as UserStringTemplate;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "UserStringDefinition"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Json documents (.json)|*.json"; // Filter files by extension
            //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dlg.InitialDirectory = Settings.DefaultTemplatePath;

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                string jsonString = JsonConvert.SerializeObject(CurrentTemplate);
                File.WriteAllText(filename, jsonString);
            }

        }

        public event EventHandler CanExecuteChanged;

        public SaveTemplateCommand(UserStringTemplate currentTemplate)
        {
            this._currentTemplate = currentTemplate;
        }

    }
}

