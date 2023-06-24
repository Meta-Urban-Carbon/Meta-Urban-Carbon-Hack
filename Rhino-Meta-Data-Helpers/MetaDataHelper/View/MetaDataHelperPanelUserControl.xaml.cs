using System.Windows.Controls;

namespace MetaDataHelper
{
    /// <summary>
    /// Interaction logic for MetaDataHelperPanelUserControl.xaml
    /// </summary>
    public partial class MetaDataHelperPanelUserControl : UserControl
    {
        public MetaDataHelperPanelUserControl()
        {
            InitializeComponent();
        }

        private ViewModel ViewModel => DataContext as ViewModel;
    }
}
