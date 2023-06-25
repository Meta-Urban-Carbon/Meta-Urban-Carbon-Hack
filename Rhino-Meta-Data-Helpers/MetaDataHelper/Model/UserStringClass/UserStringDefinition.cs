using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MetaDataHelper.UserStringClass
{
    /// <summary>
    /// Defines a UserString Key and contrins the value's respective
    /// options if it is of <see cref="UserStringValueType"/> 'Select'
    /// </summary>
    public class UserStringDefinition
    {
        private UserStringValueOptions _options = null;

        public String Key { get; set; }

        public String DefaultValue { get; set; }

        public String Value { get; set; }

        public UserStringValueType ValueType { get; set; }

        public UserStringValueOptions GetOptions()
        {
            if (this.ValueType == UserStringValueType.Select)
            {
                return this._options;
            }
            else
            {
                return null;
            }
        }

        public UserStringDefinition(string key)
        {
            this.Key = key;
            this.Value = "";
            this.ValueType = UserStringValueType.String;
            this._options = new UserStringValueOptions();
        }

        public UserString GetCurrentUserString()
        {
            return new UserString(this.Key, this.Value);
        }

        public void AddOption(String option)
        {
            this._options.AddOption(option);
        }

        public void Assign(RhinoDoc doc, Guid guid)
        {
            var docObject = doc.Objects.FindId(guid);
            var userString = new UserString(this.Key, this.Value);
            docObject.Attributes.SetUserString(userString.Key, userString.Value.ToString());
        }
    }
}
