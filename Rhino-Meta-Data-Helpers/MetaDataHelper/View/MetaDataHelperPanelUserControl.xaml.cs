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
            DataContext = new ViewModel(documentSerialNumber);
            InitializeComponent();
            Instance = this;
        }

        private ViewModel ViewModel => DataContext as ViewModel;

        //private ViewModel ViewModel => DataContext as ViewModel;
    }
}
