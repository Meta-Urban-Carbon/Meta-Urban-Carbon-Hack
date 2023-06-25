using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Eto.Forms;
using MetaDataHelper.UserStringClass;
using Newtonsoft.Json;
using RhinoWindows.Input;

namespace MetaDataHelper
{
    public class ViewModel : Rhino.UI.ViewModel, INotifyPropertyChanged
    {
        private UserStringTemplate _currentTemplate;

        public UserStringTemplate CurrentTemplate
        {
            get { return _currentTemplate; }
            set
            {
                if (_currentTemplate != value)
                {
                    _currentTemplate = value;
                    OnPropertyChanged("CurrentTemplate"); // Notify the view about the change.
                }
            }
        }

        public ICommand AddUserStringDefinitionCommand { get; set; }

        private RelayCommand<UserStringDefinition> _addSelectionOptionCommand;

        private void AddOption(UserStringDefinition definition)
        {
            if (definition != null && definition.ValueType == UserStringValueType.Select)
            {
                var dialog = new InputDialogSimple("Enter new option");
                if (dialog.ShowDialog() == true)
                {
                    string newOption = dialog.ResponseText;
                    if (!string.IsNullOrEmpty(newOption))
                    {
                        definition.ValueOptions.AddOption(newOption);
                    }
                }
            }
        }


        public ICommand OpenOptionManagerCommand { get; set; }
        public ICommand AddSelectionOptionCommand { get; set; }
        public ICommand RemoveOptionCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }


        public ViewModel(uint documentSerialNumber)
        {
            this.DocumentRuntimeSerialNumber = documentSerialNumber;
            MetaDataHelperPlugin.Instance.SettingsSaved +=
                (sender, args) => SettingsChangedMessage = $"SettingSaved {++m_saved_count}";
            m_use_multiple_counters = this.UseMultipleCounters == true;
            Rhino.UI.Panels.Show += OnShowPanel;

            this.Message = "View Model Has Loaded";
            this.CurrentTemplate = new UserStringTemplate();
            this.AddUserStringDefinitionCommand = new AddUserStringDefinitionCommand(this.CurrentTemplate);

            OpenOptionManagerCommand = new RelayCommand<UserStringDefinition>(
                definition => {
                    if (definition != null && definition.ValueType == UserStringValueType.Select)
                    {
                        var dialog = new OptionManagerDialog(definition);
                        dialog.ShowDialog();
                    }
                });

            this.SaveCommand = new RelayCommand(SaveUserStringDefinition);
            this.LoadCommand = new RelayCommand(LoadUserStringDefinition);
        }


        private void SaveUserStringDefinition()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "UserStringDefinition"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Json documents (.json)|*.json"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                string jsonString = JsonConvert.SerializeObject(this.CurrentTemplate);
                File.WriteAllText(filename, jsonString);
            }
        }

        private void LoadUserStringDefinition()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "Json documents (.json)|*.json"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                string jsonString = File.ReadAllText(filename);
                this.CurrentTemplate = JsonConvert.DeserializeObject<UserStringTemplate>(jsonString);
            }
        }


        private void OnShowPanel(object sender, Rhino.UI.ShowPanelEventArgs e)
        {
            var sn = e.DocumentSerialNumber;
            // TOOD...
        }

        private int m_saved_count;

        private uint DocumentRuntimeSerialNumber { get; }

        public string Message
        {
            get => m_message ?? string.Empty;
            set => SetProperty(value, ref m_message);
        }
        private string m_message;

        public string SettingsChangedMessage
        {
            get => m_settings_changed_message ?? string.Empty;
            set => SetProperty(value, ref m_settings_changed_message);
        }
        private string m_settings_changed_message;

        public bool? UseMultipleCounters
        {
            get => Settings.GetBool(nameof(UseMultipleCounters), false);
            set
            {
                m_use_multiple_counters = UseMultipleCounters == true;
                SetProperty(value == true, ref m_use_multiple_counters);
            }
        }
        private bool m_use_multiple_counters;

        public void IncrementCounter() => Counter = Counter + 1;

        public Rhino.PersistentSettings Settings => MetaDataHelperPlugin.Instance.Settings;

        public int Counter
        {
            get => Settings.GetInteger(nameof(Counter), 0);
            set => Settings.SetInteger(nameof(Counter), value);
        }

        public void IncrementCounter(int index) => SetCounter(index, GetCounter(index) + 1);

        public string GetKey(int index) => $"Counter{index}";

        public int GetCounter(int index) => Settings.GetInteger(GetKey(index), 0);

        public void SetCounter(int index, int value) => Settings.SetInteger(GetKey(index), value);

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Implements Property Change event handler
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}