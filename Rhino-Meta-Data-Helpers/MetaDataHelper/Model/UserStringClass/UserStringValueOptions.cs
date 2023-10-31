using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MetaDataHelper.UserStringClass
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

    public class OptionWrapper : INotifyPropertyChanged
    {
        private string _option;
        public string Option
        {
            get => _option;
            set
            {
                if (_option != value)
                {
                    _option = value;
                    OnPropertyChanged();
                }
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