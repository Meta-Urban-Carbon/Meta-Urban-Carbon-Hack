using System;
using System.IO;
using System.Windows.Input;
using System.Windows;
using Rhino;

namespace MetaDataHelper
{
    /// <summary>
    /// Loads a UserStringTemplate from JSON
    /// To the VM's Current template.
    /// </summary>
    internal class LoadTemplateCommand : ICommand
    {
        private readonly UserStringTemplate _currentTemplate;
        private readonly SavedTemplates _savedTemplates;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json", // Default file extension
                Filter = "Json documents (.json)|*.json", // Filter files by extension
                InitialDirectory = Settings.DefaultTemplatePath //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            };

            // Show open file dialog box
            bool? result = dlg.ShowDialog();
            if (result != true) return;

            // Process open file dialog box results
            string filename = dlg.FileName;
            if (!File.Exists(filename))
            {
                MessageBox.Show($"The selected file does not exist:\n{filename}", "Template Load Error");
                return;
            }

            try
            {
                var loadedTemplate = new UserStringTemplate();
                loadedTemplate.LoadAndReplace(filename);

                this._savedTemplates.Add(loadedTemplate);
                this._currentTemplate.Replace(loadedTemplate);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Failed to load template from '{filename}': {ex}");
                MessageBox.Show($"Failed to load template:\n{ex.Message}", "Template Load Error");
            }
        }

        public event EventHandler CanExecuteChanged;

        public LoadTemplateCommand(UserStringTemplate currentTemplate, SavedTemplates savedTemplates)
        {
            this._currentTemplate = currentTemplate ?? throw new ArgumentNullException(nameof(currentTemplate));
            this._savedTemplates = savedTemplates ?? throw new ArgumentNullException(nameof(savedTemplates));
        }
    }
}
   
