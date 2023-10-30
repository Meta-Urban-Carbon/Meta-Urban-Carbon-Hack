using MetaDataHelper.UserStringClass;
using System.Windows;
using System.Windows.Controls;

namespace MetaDataHelper
{
    /// <summary>
    /// Interaction logic for OptionManagerDialog.xaml
    /// </summary>
    public partial class OptionManagerDialog : Window
    {
        private UserStringDefinition _userStringDefinition;
        public OptionManagerDialog(UserStringDefinition userStringDefinition)
        {
            InitializeComponent();
            _userStringDefinition = userStringDefinition;
            this.DataContext = _userStringDefinition;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string newOption = NewOptionTextBox.Text;
            if (!string.IsNullOrEmpty(newOption))
            {
                _userStringDefinition.AddOption(newOption);
            }
        }
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var option = (sender as Button).DataContext as string;
            if (option != null)
            {
                _userStringDefinition.RemoveOption(option);
            }
        }
    }
}
