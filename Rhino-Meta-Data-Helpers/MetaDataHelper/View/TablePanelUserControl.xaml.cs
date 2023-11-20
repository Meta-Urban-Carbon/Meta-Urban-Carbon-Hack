using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;


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

            var viewModel = new TableViewModel(documentSerialNumber);
            DataContext = viewModel;

            // Subscribe to the ColumnsUpdated event
            viewModel.ColumnsUpdated += OnColumnsUpdated;

            Instance = this;
        }

        private TableViewModel ViewModel => DataContext as TableViewModel;

        //private ViewModel ViewModel => DataContext as ViewModel;

        private void DataGridAttributes_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TableViewModel;
            if (viewModel != null)
            {
                CreateDataGridColumns(DataGridAttributes, viewModel.TableAttributes.AllObjectKeys);
            }
        }

        private void OnColumnsUpdated(IEnumerable<string> allKeys)
        {
            CreateDataGridColumns(DataGridAttributes, allKeys);
        }

        public void CreateDataGridColumns(DataGrid dataGrid, IEnumerable<string> allKeys)
        {
            // Identify the starting index for adding new columns
            // This assumes that any static columns defined in XAML are already present
            int startIndex = dataGrid.Columns.Count;

            foreach (var key in allKeys)
            {
                // Check if a column for this key already exists, to avoid duplicates
                if (dataGrid.Columns.Any(c => c.Header.ToString().Equals(key)))
                    continue;

                var column = new DataGridTextColumn
                {
                    Header = key,
                    Binding = new Binding($"Attributes[{key}]") { Converter = new DictionaryValueConverter() }
                };

                dataGrid.Columns.Insert(startIndex++, column);
            }
        }


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
