using Rhino;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MetaDataHelper.UserStringClass
{
    /// <summary>
    /// Defines a UserString Key and contrins the value's respective
    /// options if it is of <see cref="UserStringValueType"/> 'Select'
    /// </summary>
    public class UserStringDefinition : INotifyPropertyChanged
    {

        private String _key;
        private String _defaultValue;
        private String _value;
        private UserStringValueType _type;
        private UserStringValueOptions _options = null;

        public String Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }

        public String DefaultValue { get=>_defaultValue;
            set
            {
                _defaultValue = value;
                OnPropertyChanged();
            }
        }

        public String Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public UserStringValueType ValueType
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

        public UserStringValueOptions ValueOptions
        {
            get => _options;
            set
            {
                _options = value;
                OnPropertyChanged();
            }
        }

        public UserStringDefinition(string key)
        {
            this.Key = key;
            this.Value = "value";
            this.ValueType = UserStringValueType.String;
            this.ValueOptions = new UserStringValueOptions();
            this.ValueOptions.AddOption(this.Value);
        }

        public UserString GetCurrentUserString()
        {
            return new UserString(this.Key, this.Value);
        }

        public void AddOption(String option)
        {
            this._options.AddOption(option);
        }

        public void RemoveOption(String option)
        {
            this._options.RemoveOption(option);
        }

        public void Assign(RhinoDoc doc, Guid guid)
        {
            var docObject = doc.Objects.FindId(guid);
            var userString = new UserString(this.Key, this.Value);
            docObject.Attributes.SetUserString(userString.Key, userString.Value.ToString());
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
