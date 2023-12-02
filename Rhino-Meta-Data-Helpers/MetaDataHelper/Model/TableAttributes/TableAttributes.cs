using Rhino;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MetaDataHelper
{
    /// <summary>
    /// This is the list of all attributes that is used to
    /// populate the table. It is a collection of <see cref="RhinoObjectTableAttributes"/>
    /// 
    /// </summary>
    public class TableAttributes : ObservableCollection<RhinoObjectTableAttributes>, INotifyPropertyChanged
    {

        private List<string> _allObjectKeys; // List of all object keys

        public List<string> AllObjectKeys
        {
            get { return _allObjectKeys; }
            set
            {
                _allObjectKeys = value;
                OnPropertyChanged();
            }
        }

        public TableAttributes()
        {
            _allObjectKeys = new List<string>();
            //_allObjectKeys = new List<string> { "ObjectGuid" };
            this.AddAllDocumentObjects();
            this.CreateAllObjectKeys();
        }

        public void Add(RhinoObject rhinoObject)
        {
            //Create a new RhinoObjectTableAttributes object
            var rhinoObjectTableAttributes = new RhinoObjectTableAttributes(rhinoObject);

            //check AllObjectKeys for objects UserString keys and update as needed
            UpdateAllObjectKeys(rhinoObject);

            //Add the new RhinoObjectTableAttributes object to the collection
            base.Add(rhinoObjectTableAttributes);
        }

        public void AddAllDocumentObjects()
        {
            var doc = RhinoDoc.ActiveDoc;
            var rhinoObjects = doc.Objects;

            foreach (var rhinoObject in rhinoObjects)
            {
                var rhinoObjectTableAttributes = new RhinoObjectTableAttributes(rhinoObject);
                Add(rhinoObjectTableAttributes);
            }
        }

        //update AllObjectKeys with new object keys
        private void UpdateAllObjectKeys(RhinoObject rhinoObject)
        {
            var rhinoObjectKeys = rhinoObject.Attributes.GetUserStrings().Keys;
            foreach (var key in rhinoObjectKeys)
            {
                if (!_allObjectKeys.Contains(key))
                {
                    _allObjectKeys.Add(key.ToString());
                    OnPropertyChanged(nameof(AllObjectKeys));
                }
            }
        }

        //Create AllObjectKeys from all RhinoObjectTableAttributes objects in the collection
        private void CreateAllObjectKeys()
        {
            foreach (var rhinoObjectTableAttributes in this)
            {
                foreach (var key in rhinoObjectTableAttributes.Attributes.Keys)
                {
                    if (!_allObjectKeys.Contains(key))
                    {
                        _allObjectKeys.Add(key);
                    }
                }
            }
        }

        //Refresh / Update the entire collection from the current rhino document. 
        //Basicly a full re-build the collection
        public void Refresh()
        {
            _allObjectKeys = new List<string>();
            this.Clear();
            this.AddAllDocumentObjects();
            this.CreateAllObjectKeys();
        }

        // Method to compute filters (example method, implement as needed)
        public void ApplyFilter(string filterCriteria)
        {
            // Filtering logic goes here
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
