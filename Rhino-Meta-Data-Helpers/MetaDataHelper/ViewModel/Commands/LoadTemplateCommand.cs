using MetaDataHelper.UserStringClass;
using System;
using System.Windows.Input;

namespace MetaDataHelper
{
    /// <summary>
    /// Loads a UserStringTemplate from JSON
    /// To the VM's Current template.
    /// </summary>
    internal class LoadTemplateCommand : ICommand
    {
        private UserStringTemplate _currentTemplate;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Json documents (.json)|*.json"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Load document
                string filename = dlg.FileName;
                this._currentTemplate.LoadAndReplace(filename);
            }
        }

        public event EventHandler CanExecuteChanged;

        public LoadTemplateCommand(UserStringTemplate currentTemplate)
        {
            this._currentTemplate = currentTemplate;
        }
    }
}
   