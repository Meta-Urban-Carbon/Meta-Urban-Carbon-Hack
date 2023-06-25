using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Rhino;

namespace MetaDataHelper.UserStringClass
{
    public class UserStringTemplate : ObservableCollection<UserStringDefinition>, INotifyPropertyChanged
    {

        public void Assign(RhinoDoc doc, Guid guid)
        {
            var docObject = doc.Objects.FindId(guid);

            foreach (var UserStringDeffinition in this)
            {
                var userString = UserStringDeffinition.GetCurrentUserString();
                docObject.Attributes.SetUserString(userString.Key, userString.Value.ToString());
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
