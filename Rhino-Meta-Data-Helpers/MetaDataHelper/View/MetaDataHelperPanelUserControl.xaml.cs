using MetaDataHelper.UserStringClass;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MetaDataHelper
{
    /// <summary>
    /// Interaction logic for MetaDataHelperPanelUserControl.xaml
    /// </summary>
    public partial class MetaDataHelperPanelUserControl : UserControl
    {
        public static MetaDataHelperPanelUserControl Instance;
        public MetaDataHelperPanelUserControl(uint documentSerialNumber)
        {
            InitializeComponent();
            DataContext = new ViewModel(documentSerialNumber);
            Instance = this;
        }

        private ViewModel ViewModel => DataContext as ViewModel;

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

    public class ValueTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate SelectOptionsTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            UserStringDefinition definition = item as UserStringDefinition;

            if (definition != null)
            {
                if (definition.ValueType == UserStringValueType.String)
                {
                    return StringTemplate;
                }
                else if (definition.ValueType == UserStringValueType.Select)
                {
                    return SelectOptionsTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
