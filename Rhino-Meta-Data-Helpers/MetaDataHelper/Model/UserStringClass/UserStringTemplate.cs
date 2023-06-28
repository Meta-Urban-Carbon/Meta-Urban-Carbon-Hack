using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Rhino;

namespace MetaDataHelper.UserStringClass
{
    public class UserStringTemplate : ObservableCollection<UserStringDefinition>, INotifyPropertyChanged
    {
        private string _name; 

        private string _description;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public void Assign(RhinoDoc doc, Guid guid)
        {
            var docObject = doc.Objects.FindId(guid);

            foreach (var UserStringDeffinition in this)
            {
                var userString = UserStringDeffinition.GetCurrentUserString();
                docObject.Attributes.SetUserString(userString.Key, userString.Value.ToString());
            }
        }

        /// <summary>
        /// Loads a new UserStringTemplate from JSON
        /// and replaces this instance
        /// the loaded content.
        /// </summary>
        /// <param name="filename"></param>
        public void LoadAndReplace(string filename)
        {
            string jsonString = File.ReadAllText(filename);
            //var loadedTemplate = JsonConvert.DeserializeObject<List<UserStringDefinition>>(jsonString);
            var loadedTemplate = JsonConvert.DeserializeObject<UserStringTemplate>(jsonString);
            
            this.Clear();

            this.Name = loadedTemplate.Name;
            this.Description = loadedTemplate.Description; 

            foreach (var userStringDefinition in loadedTemplate)
            {
                this.Add(userStringDefinition);
            }
        }

        /// <summary>
        /// Loads a UserStringTemplate from JSON
        /// and add's its UserStringDefinitions to
        /// This instance.
        /// </summary>
        /// <param name="filename"></param>
        public void LoadAndExtend(string filename)
        {
            string jsonString = File.ReadAllText(filename);
            //var loadedTemplate = JsonConvert.DeserializeObject<List<UserStringDefinition>>(jsonString);
            var loadedTemplate = JsonConvert.DeserializeObject<UserStringTemplate>(jsonString);

            foreach (var userStringDefinition in loadedTemplate)
            {
                this.Add(userStringDefinition);
            }
        }

        public void Replace(UserStringTemplate template)
        {
            this.Clear();
            foreach (var userStringDefinition in template)
            {
                this.Add(userStringDefinition);
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
