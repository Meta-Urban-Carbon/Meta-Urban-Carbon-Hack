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


        /// <summary>
        /// Adds a new option to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string newOption = NewOptionTextBox.Text;
            if (!string.IsNullOrEmpty(newOption))
            {
                _userStringDefinition.AddOption(newOption);
            }
        }

        /// <summary>
        /// Removes a option from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var option = (sender as Button).DataContext as string;
            if (option != null)
            {
                _userStringDefinition.RemoveOption(option);
            }
        }
        /// <summary>
        /// Closes this dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
