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

        public void Add(UserStringTemplate userStringTemplate)
        {
            //check if name is unique
            if (this.checkDuplicates(userStringTemplate.Name))
            {
                base.Add(userStringTemplate);
            }
            else
            {
                //replace existing
                foreach (var item in this)
                {
                    if (item.Name == userStringTemplate.Name)
                    {
                        item.Replace(userStringTemplate);
                    }
                }
            }
        }

        /// <summary>
        /// checks if the name is unique
        /// </summary>
        /// <param name="name"></param>
        /// <returns>is it unique? bool</returns>
        public bool checkDuplicates(string name)
        {
            foreach (var item in this)
            {
                if (item.Name == name)
                {
                    return false;
                }
            }
            return true;
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
