using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;

namespace MetaDataHelper
{
    internal class ExportCsvCommand : ICommand
    {
        private readonly TableViewModel _vm;
        public ExportCsvCommand(TableViewModel vm) { _vm = vm ?? throw new ArgumentNullException(nameof(vm)); }

        public bool CanExecute(object parameter) => true;
        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var rows = _vm.TableAttributes?.ToList() ?? new List<RhinoObjectTableAttributes>();
            var headers = (_vm.TableAttributes?.AllObjectKeys ?? new List<string>()).ToList();
            if (rows.Count == 0 && !headers.Contains("ObjectGuid")) headers.Add("ObjectGuid");

            var dlg = new SaveFileDialog
            {
                Title = "Export table to CSV",
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "RhinoTable.csv",
                AddExtension = true,
                DefaultExt = ".csv",
                OverwritePrompt = true
            };

            if (dlg.ShowDialog() != true) return;

            using (var writer = new StreamWriter(dlg.FileName))
            {
                writer.WriteLine(string.Join(",", headers.Select(CsvEscape)));
                foreach (var row in rows)
                {
                    var line = new List<string>(headers.Count);
                    foreach (var key in headers)
                    {
                        string value;
                        if (!row.Attributes.TryGetValue(key, out value))
                            value = string.Equals(key, "ObjectGuid", StringComparison.Ordinal) ? row.ObjectGuid.ToString() : string.Empty;
                        line.Add(CsvEscape(value ?? string.Empty));
                    }
                    writer.WriteLine(string.Join(",", line));
                }
            }
        }

        private static string CsvEscape(string s)
        {
            if (s == null) return string.Empty;
            bool mustQuote = s.Contains(",") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r");
            var escaped = s.Replace("\"", "\"\"");
            return mustQuote ? $"\"{escaped}\"" : escaped;
        }
    }
}
