﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Newtonsoft.Json;
using Rhino;

namespace MetaDataHelper
{
    /// <summary>
    /// This is the basic template or metadata class object.
    /// it holds a general name and description, and each
    /// user string deffintion is held in it's collection.
    /// It has helper tools for loading, adding and replacing
    /// items. 
    /// </summary>
    public class UserStringTemplate : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private ObservableCollection<UserStringDefinition> _collection;

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

        /// <summary>
        /// The Collection of <see cref="UserStringDefinition"/>s that
        /// make up this template. 
        /// </summary>
        public ObservableCollection<UserStringDefinition> Collection
        {
            get => _collection;
            set
            {
                _collection = value;
                OnPropertyChanged();
            }
        }

        public ICommand RemoveCommand { get; private set; }


        public UserStringTemplate()
        {
            this.Collection = new ObservableCollection<UserStringDefinition>();
            this.RemoveCommand = new RelayCommand(ExecuteRemoveCommand, CanExecuteRemoveCommand);
        }

        /// <summary>
        /// Assigns this template to a specific rhino geometry. 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="guid"></param>
        public void Assign(RhinoDoc doc, Guid guid)
        {
            var docObject = doc.Objects.FindId(guid);

            foreach (var UserStringDeffinition in this.Collection)
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
            
            this.Collection.Clear();

            this.Name = loadedTemplate.Name;
            this.Description = loadedTemplate.Description; 

            foreach (var userStringDefinition in loadedTemplate.Collection)
            {
                this.Collection.Add(userStringDefinition);
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

            this.Name = loadedTemplate.Name;
            this.Description = loadedTemplate.Description;

            foreach (var userStringDefinition in loadedTemplate.Collection)
            {
                this.Collection.Add(userStringDefinition);
            }
        }

        public void Replace(UserStringTemplate template)
        {
            this.Name = template.Name;
            this.Description = template.Description;
            this.Collection.Clear();
            foreach (var userStringDefinition in template.Collection)
            {
                this.Collection.Add(userStringDefinition);
            }
        }

        public void Add(UserStringDefinition template)
        {
            this.Collection.Add(template);
        }

        public void AddRange(List<UserStringDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                this.Collection.Add(definition);
            }
        }

        public void Remove(UserStringDefinition definition)
        {
            this.Collection.Remove(definition);
        }

        private bool CanExecuteRemoveCommand(object parameter)
        {
            return parameter is UserStringDefinition;
        }

        private void ExecuteRemoveCommand(object parameter)
        {
            if (parameter is UserStringDefinition definition)
            {
                this.Remove(definition);
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
