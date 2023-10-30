using MetaDataHelper.UserStringClass;
using System.Windows;
using System.Windows.Controls;

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
