using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace MetaDataHelper.UserStringClass
{
    public class UserStringType: UserString
    {
        private UserStringValueOptions options = null;

        public UserStringType(string key, UserStringValueType type)
        {
            this.Key = key;
            this.ValueType = type;
            if (type == UserStringValueType.Select)
            {
                this.options = new UserStringValueOptions();
            }
        }

        public void Assign(RhinoDoc doc, Guid guid)
        {
            var docObject = doc.Objects.FindId(guid);

            foreach (var UserString in this)
            {
                docObject.Attributes.SetUserString(UserString.Key, UserString.Value.ToString());
            }

        }
    }
}
