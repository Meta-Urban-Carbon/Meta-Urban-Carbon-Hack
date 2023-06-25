using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaDataHelper.UserStringClass
{
    public class UserString
    {
        /// <summary>
        /// The Key for User String
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Value For User String
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The Type of the Value for UserString <see cref="UserStringValueType"/>
        /// </summary>
        public UserStringValueType ValueType { get; set; }

        public void Assign(Guid guid)
        {
            var doc = RhinoDoc.ActiveDoc;
            var docObject = doc.Objects.FindId(guid);
            docObject.Attributes.SetUserString(this.Key, this.Value.ToString());
        }
    }
}
