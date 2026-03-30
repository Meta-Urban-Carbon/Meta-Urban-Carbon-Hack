using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Rhino;

namespace MetaDataHelper
{
    internal class SaveTemplateCommand: ICommand
    {

        private readonly UserStringTemplate _currentTemplate;
        private readonly SavedTemplates _savedTemplates;

        public bool CanExecute(object parameter)
        {
            return true;
            //throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            var currentTemplate = parameter as UserStringTemplate ?? _currentTemplate;
            if (currentTemplate == null)
            {
                MessageBox.Show("No template is available to save.", "Template Save Error");
                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = string.IsNullOrWhiteSpace(currentTemplate.Name) ? "UserStringTemplate" : currentTemplate.Name;//"UserStringDefinition"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Json documents (.json)|*.json"; // Filter files by extension
            //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            dlg.InitialDirectory = Settings.DefaultTemplatePath;

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                try
                {
                    // Save document
                    string filename = dlg.FileName;
                    string jsonString = JsonConvert.SerializeObject(currentTemplate);
                    File.WriteAllText(filename, jsonString);
                    this._savedTemplates.Add(currentTemplate);
                }
                catch (Exception ex)
                {
                    RhinoApp.WriteLine($"Failed to save template: {ex}");
                    MessageBox.Show($"Failed to save template:\n{ex.Message}", "Template Save Error");
                }
            }

        }

        public event EventHandler CanExecuteChanged;

        public SaveTemplateCommand(UserStringTemplate currentTemplate, SavedTemplates savedTemplates)
        {
            this._currentTemplate = currentTemplate ?? throw new ArgumentNullException(nameof(currentTemplate));
            this._savedTemplates = savedTemplates ?? throw new ArgumentNullException(nameof(savedTemplates));
        }

    }
}
