using System.Collections.ObjectModel;
using System.Linq;

namespace MetaDataHelper
{
    public class UserStringValueOptions : ObservableCollection<OptionWrapper>
    {
        public void AddOption(string keyOption)
        {
            this.Add(new OptionWrapper { Option = keyOption });
        }

        public void AddOption(int keyOption)
        {
            this.Add(new OptionWrapper { Option = keyOption.ToString() });
        }

        public void AddOption(double keyOption)
        {
            this.Add(new OptionWrapper { Option = keyOption.ToString() });
        }

        public void RemoveOption(string option)
        {
            var itemToRemove = this.FirstOrDefault(ow => ow.Option == option);
            if (itemToRemove != null)
            {
                this.Remove(itemToRemove);
            }
        }

        // Check if an option exists in the collection
        public bool ContainsOption(string option)
        {
            return this.Any(ow => ow.Option == option);
        }

        // Get the first option if it exists
        public string GetFirstOption()
        {
            return this.FirstOrDefault()?.Option ?? string.Empty;
        }
    }
}