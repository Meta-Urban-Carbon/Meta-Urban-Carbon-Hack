using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaDataHelper
{
    /// <summary>
    /// Interaction logic for MetaDataHelperPanelUserControl.xaml
    /// </summary>
    public partial class TablePanelUserControl : UserControl
    {
        public static TablePanelUserControl Instance;
        public TablePanelUserControl(uint documentSerialNumber)
        {
            InitializeComponent();
            DataContext = new TableViewModel(documentSerialNumber);
            Instance = this;
        }

        private TableViewModel ViewModel => DataContext as TableViewModel;

        //private ViewModel ViewModel => DataContext as ViewModel;


        private void DoubleValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+"); // Adjusted pattern to also allow dots for decimal and negative sign
            e.Handled = regex.IsMatch(e.Text);
        }

        private void IntergerValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9-]+"); // Adjusted pattern to also allow negative sign
            e.Handled = regex.IsMatch(e.Text);
        }   

        private void TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true; // Disallow pasting in the TextBox
            }
        }
    }

}
