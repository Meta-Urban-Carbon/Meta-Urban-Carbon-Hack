using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.DocObjects;

namespace MetaDataHelper
{
    /// <summary>
    /// Holds the attributes of a RhinoObject
    /// Attributes are <see cref="Rhino.DocObjects.ObjectAttributes"/>
    /// in dicutionary
    /// </summary>
    public class RhinoObjectTableAttributes
    {
            public Dictionary<string, string> Attributes { get; private set; }

            public Guid ObjectGuid { get; private set; }

            /// <summary>
            /// A Rhino object the will set this instances attributes
            /// and guid.
            /// </summary>
            /// <param name="rhinoObject"></param>
            public RhinoObjectTableAttributes(RhinoObject rhinoObject)
            {
                //Assign the object userStrings to the Attributes dictionary
                Attributes = new Dictionary<string, string>();

                var userStrings = rhinoObject.Attributes.GetUserStrings();
                foreach (var key in userStrings.AllKeys)
                {
                    Attributes[key] = userStrings[key];
                }
                //assign the object guid
                ObjectGuid = rhinoObject.Id;
                Attributes["ObjectGuid"] = userStrings[rhinoObject.Id.ToString()];
            }
    }
}