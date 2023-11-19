using Rhino;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MetaDataHelper
{

    public class TableAttributes : DynamicObject, INotifyPropertyChanged
    {
        private Dictionary<string, string> _attributes = new Dictionary<string, string>();

        public Dictionary<string, string> Attributes
        {
            get => _attributes;
            set
            {
                _attributes = value;
                OnPropertyChanged();
            }
        }

        // Implement the dynamic behavior for getting and setting properties
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_attributes.TryGetValue(binder.Name, out var value))
            {
                result = value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }


        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _attributes[binder.Name] = (string)value;
            OnPropertyChanged(binder.Name);
            return true;
        }

        // Method to update attributes
        /*public void UpdateAttributes(RhinoObject rhinoObject)
        {
            // Clear existing attributes
            Attributes.Clear();

            // Extract user strings from the Rhino object and update the Attributes dictionary
            var userStrings = rhinoObject.Attributes.GetUserStrings();
            foreach (var key in userStrings.AllKeys)
            {
                Attributes[key] = userStrings[key];
            }
        }*/

        public void UpdateAttributes(RhinoObject rhinoObject)
        {
            var userStrings = rhinoObject.Attributes.GetUserStrings();
            foreach (var key in userStrings.AllKeys)
            {
                this.TrySetMember(new SetMemberBinderImpl(key), userStrings[key]);
            }
        }


        // Method to compute filters (example method, implement as needed)
        public void ApplyFilter(string filterCriteria)
        {
            // Filtering logic goes here
        }

        // Static method to scrape all user string info from the Rhino document
        public static List<TableAttributes> GetAllObjectsAttributes(RhinoDoc doc)
        {
            var allAttributes = new List<TableAttributes>();

            foreach (var obj in doc.Objects)
            {
                var tableAttributes = new TableAttributes();
                tableAttributes.UpdateAttributes(obj);
                allAttributes.Add(tableAttributes);
            }

            // Handling nulls for missing attributes
            var allKeys = allAttributes.SelectMany(attr => attr.Attributes.Keys).Distinct();
            foreach (var attr in allAttributes)
            {
                foreach (var key in allKeys)
                {
                    if (!attr.Attributes.ContainsKey(key))
                    {
                        attr.Attributes[key] = null; // Insert nulls for missing attributes
                    }
                }
            }

            return allAttributes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Implements Property Change event handler
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class SetMemberBinderImpl : SetMemberBinder
        {
            public SetMemberBinderImpl(string name) : base(name, false) { }

            public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
            {
                // Implementation details here
                return null;
            }
        }

    }




}
