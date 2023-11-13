using Rhino;
using System;

namespace MetaDataHelper
{
    public class UserString
    {
        public UserString(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// The Key for User String
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The Value For User String
        /// </summary>
        public string Value { get; set; }

        public void Assign(Guid guid)
        {
            var doc = RhinoDoc.ActiveDoc;
            var docObject = doc.Objects.FindId(guid);
            docObject.Attributes.SetUserString(this.Key, this.Value.ToString());
        }
    }
}
