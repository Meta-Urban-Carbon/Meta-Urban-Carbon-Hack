using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaDataHelper.UserStringClass
{
    public class UserStringValueOptions: ObservableCollection<String>
    {
        public void AddOption(String keyOption)
        {
            this.Add(keyOption);
        }

        public void AddOption(int keyOption)
        {
            this.Add(keyOption.ToString());
        }

        public void AddOption(double keyOption)
        {
            this.Add(keyOption.ToString());
        }
    }
}
