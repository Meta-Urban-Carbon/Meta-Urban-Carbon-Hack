using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using Rhino;


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

            // Clean up on unload to avoid leaks
            this.Unloaded += TablePanelUserControl_Unloaded;

            Instance = this;
        }

        private void TablePanelUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TableViewModel vm)
            {
                vm.ColumnsUpdated -= OnColumnsUpdated;
                vm.Dispose();
            }
        }

        private TableViewModel ViewModel => DataContext as TableViewModel;

        private void DataGridAttributes_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TableViewModel;
            if (viewModel != null)
            {
                // Ensure initial data is loaded
                viewModel.UpdateAttributes(null);
                CreateDataGridColumns(DataGridAttributes, viewModel.TableAttributes.AllObjectKeys);
            }
        }

        private void OnColumnsUpdated(IEnumerable<string> allKeys)
        {
            CreateDataGridColumns(DataGridAttributes, allKeys);
        }

        public void CreateDataGridColumns(DataGrid dataGrid, IEnumerable<string> allKeys)
        {
            if (dataGrid == null) return;

            var keys = (allKeys ?? Enumerable.Empty<string>())
                .Distinct(StringComparer.Ordinal)
                .ToHashSet(StringComparer.Ordinal);

            // Remove stale dynamic columns (keep the static "Select" column)
            for (int i = dataGrid.Columns.Count - 1; i >= 0; i--)
            {
                var col = dataGrid.Columns[i];
                var header = col.Header?.ToString();
                if (!string.Equals(header, "Select", StringComparison.Ordinal))
                {
                    if (header == null || !keys.Contains(header))
                        dataGrid.Columns.RemoveAt(i);
                }
            }

            // Insert after the "Select" column if present
            int startIndex = 0;
            var selectIndex = dataGrid.Columns.ToList().FindIndex(c => string.Equals(c.Header?.ToString(), "Select", StringComparison.Ordinal));
            startIndex = selectIndex >= 0 ? selectIndex + 1 : dataGrid.Columns.Count;

            foreach (var key in keys)
            {
                // Avoid duplicates
                if (dataGrid.Columns.Any(c => string.Equals(c.Header?.ToString(), key, StringComparison.Ordinal)))
                    continue;

                var column = new DataGridTextColumn
                {
                    Header = key,
                    Binding = new Binding($"Attributes[{key}]")
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay
                    }
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

        private void DataGridAttributes_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (ViewModel == null) return;
            if (!ViewModel.LiveEdit) return;
            if (e.EditAction != DataGridEditAction.Commit) return;

            var row = e.Row?.Item as RhinoObjectTableAttributes;
            if (row == null) return;

            var tb = e.EditingElement as TextBox;
            if (tb == null) return;

            // Ensure latest text is pushed to binding source
            var be = tb.GetBindingExpression(TextBox.TextProperty);
            be?.UpdateSource();

            var newValue = tb.Text;
            var key = e.Column.Header?.ToString();
            if (string.IsNullOrEmpty(key)) return;

            ViewModel.ApplyCellEdit(row, key, newValue);
        }

        private void ApplySelectedRows_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;
            var selected = DataGridAttributes.SelectedItems.Cast<RhinoObjectTableAttributes>().ToList();
            ViewModel.ApplySelectedRows(selected);
        }
    }

}
