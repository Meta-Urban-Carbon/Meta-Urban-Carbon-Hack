﻿using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MetaDataHelper
{
    public class ViewModel : Rhino.UI.ViewModel, INotifyPropertyChanged
    {
        private UserStringTemplate _currentTemplate;
        private SavedTemplates _savedTemplates;
        private UserStringTemplate _selectedSavedTemplate;
        private ObservableCollection<Layer> _layers = new ObservableCollection<Layer>();
        private Layer _selectedLayer;

        public ObservableCollection<Layer> Layers
        {
            get => _layers;
            set
            {
                _layers = value;
                OnPropertyChanged();
            }
        }

        public Layer SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value;
                OnPropertyChanged();
            }
        }

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
        public UserStringTemplate SelectedSavedTemplate
        {
            get => _selectedSavedTemplate;
            set
            {
                _selectedSavedTemplate = value;
                OnPropertyChanged();

                // If the new value is not null, replace the CurrentTemplate with it
                if (value != null)
                {
                    CurrentTemplate.Replace(_selectedSavedTemplate);
                }
            }
        }

        public SavedTemplates SavedTemplates
        {
            get { return _savedTemplates; }
            set
            {
                _savedTemplates = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddUserStringDefinitionCommand { get; set; }
        public ICommand OpenOptionManagerCommand { get; set; }
        public ICommand AddSelectionOptionCommand { get; set; }
        public ICommand RemoveOptionCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand AssignTemplateCommand { get; set; }
        public ICommand AssignTemplateToLayerCommand { get; set; }

        public ViewModel(uint documentSerialNumber)
        {
            this.DocumentRuntimeSerialNumber = documentSerialNumber;
            MetaDataHelperPlugin.Instance.SettingsSaved +=
                (sender, args) => SettingsChangedMessage = $"SettingSaved {++m_saved_count}";
            m_use_multiple_counters = this.UseMultipleCounters == true;
            Rhino.UI.Panels.Show += OnShowPanel;

            RhinoDoc.LayerTableEvent += OnLayerTableEvent;
            UpdateLayers();

            // Set the selected layer to the current layer in the active Rhino document
            var doc = Rhino.RhinoDoc.ActiveDoc;
            var currentLayerName = doc.Layers.CurrentLayer.Name;
            this.SelectedLayer = Layers.FirstOrDefault(l => l.Name == currentLayerName);

            this.Message = "View Model Has Loaded";
            this.CurrentTemplate = new UserStringTemplate();
            this.SavedTemplates = new SavedTemplates();
            if (this.SavedTemplates.Count > 0)
            {
                this.SelectedSavedTemplate = this.SavedTemplates[0];
            }

            this.AddUserStringDefinitionCommand = new AddUserStringDefinitionCommand(this.CurrentTemplate);
            this.OpenOptionManagerCommand = new OpenOptionManagerCommand(this.CurrentTemplate);
            this.SaveCommand = new SaveTemplateCommand(this.CurrentTemplate, this.SavedTemplates);
            this.LoadCommand = new LoadTemplateCommand(this.CurrentTemplate, this.SavedTemplates);
            this.AssignTemplateCommand = new AssignTemplateCommand(this.CurrentTemplate);
            this.AssignTemplateToLayerCommand = new AssignTemplateToLayerCommand(this.CurrentTemplate);

        }

        public void Dispose()
        {
            RhinoDoc.LayerTableEvent -= OnLayerTableEvent;
        }

        private void OnLayerTableEvent(object sender, LayerTableEventArgs e)
        {
            if (e.EventType == LayerTableEventType.Added ||
                e.EventType == LayerTableEventType.Deleted ||
                e.EventType == LayerTableEventType.Modified) // Handle Modified event
            {
                UpdateLayers();
            }
        }

        private void UpdateLayers()
        {
            // Temporarily store the selected layer name to restore it after updating the list
            var selectedLayerName = SelectedLayer?.Name;

            Layers.Clear();
            foreach (var layer in RhinoDoc.ActiveDoc.Layers)
            {
                Layers.Add(layer);
            }

            // Restore the selected layer
            SelectedLayer = Layers.FirstOrDefault(l => l.Name == selectedLayerName);
        }

        private void OnShowPanel(object sender, Rhino.UI.ShowPanelEventArgs e)
        {
            var sn = e.DocumentSerialNumber;
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