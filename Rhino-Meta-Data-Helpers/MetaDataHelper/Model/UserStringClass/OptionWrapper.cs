using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MetaDataHelper
{
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
