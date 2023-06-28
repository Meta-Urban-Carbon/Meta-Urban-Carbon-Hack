using MetaDataHelper.UserStringClass;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MetaDataHelper
{
    public class SavedTemplates: ObservableCollection<UserStringTemplate>, INotifyPropertyChanged
    {
        private String _name;
        private String _targetFolder;

        public String Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public String TargetFolder
        {
            get => _targetFolder;
            set
            {
                _targetFolder = value;
                OnPropertyChanged();
            }
        }

        public SavedTemplates()
        {
            this.Name = "Default";
            this.TargetFolder = Settings.DefaultTemplatePath;
            this.LoadTemplatesFromFolder(this.TargetFolder);
        }

        public void LoadTemplatesFromFolder(String targetFolder)
        {
            this.Clear();
            // Check if directory exists
            if (Directory.Exists(targetFolder))
            {
                var jsonFiles = Directory.GetFiles(targetFolder, "*.json");
                foreach (var file in jsonFiles)
                {
                    string jsonString = File.ReadAllText(file);
                    var userStringTemplate = JsonConvert.DeserializeObject<UserStringTemplate>(jsonString);
                    this.Add(userStringTemplate);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Implements Property Change event handler
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
