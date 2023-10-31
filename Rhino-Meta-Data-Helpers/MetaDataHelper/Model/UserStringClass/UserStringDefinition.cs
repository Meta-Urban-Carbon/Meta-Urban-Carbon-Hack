using Rhino;
using System;
using System.ComponentModel;
using System.Linq;
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
        private String _ghFilePath;
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
                if (_type != value)
                {
                    _type = value;
                    AdjustValueForType();
                    OnPropertyChanged();
                }
            }
        }

        public String GhFilePath
        {
            get => _ghFilePath;
            set
            {
                _ghFilePath=value;
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

        public void Assign(Guid guid)
        {
            // Check if guid, Value and Key are not null
            if (guid != null && Key != null && Value != null)
            {
                var doc = RhinoDoc.ActiveDoc;
                var docObject = doc.Objects.FindId(guid);

                var userString = new UserString(this.Key, this.Value);
                docObject.Attributes.SetUserString(userString.Key, userString.Value.ToString());
            }
            else if (guid != null && Key != null && Value == null)
            {
                var doc = RhinoDoc.ActiveDoc;
                var docObject = doc.Objects.FindId(guid);

                // If Value is null, assign an empty string to it
                var userString = new UserString(this.Key, "_");
                docObject.Attributes.SetUserString(userString.Key, userString.Value.ToString());
            }
        }

        private void AdjustValueForType()
        {
            switch (_type)
            {
                case UserStringValueType.String:
                    // For a string, we don't change the value
                    //If Option, unwrap the option
                    if (_options != null)
                    {
                        Value = _options.GetFirstOption();
                    }
                    break;

                case UserStringValueType.Integer:
                    // Try to parse the current value as an integer
                    if (!int.TryParse(_value, out int intResult))
                    {
                        Value = "0"; // Default integer value
                    }
                    else
                    {
                        Value = intResult.ToString();
                    }
                    break;

                case UserStringValueType.Double:
                    // Try to parse the current value as a double
                    if (!double.TryParse(_value, out double doubleResult))
                    {
                        Value = "0.0"; // Default double value
                    }
                    else
                    {
                        Value = doubleResult.ToString();
                    }
                    break;

                case UserStringValueType.Boolean:
                    // Try to interpret the value as a boolean
                    string lowerValue = _value.ToLower();
                    if (lowerValue == "true" || lowerValue == "yes")
                    {
                        Value = "true";
                    }
                    else if (lowerValue == "false" || lowerValue == "no")
                    {
                        Value = "false";
                    }
                    else
                    {
                        Value = "false"; // Default boolean value
                    }
                    break;

                case UserStringValueType.Select:
                    // For a selection type, we'll check if the current value is a valid option. 
                    // If not, we can default to the first option or another default value.
                    if (!_options.ContainsOption(_value))
                    {
                        Value = _options.GetFirstOption();
                    }
                    break;

                default:
                    break;
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
