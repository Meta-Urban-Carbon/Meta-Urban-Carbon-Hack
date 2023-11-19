using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace MetaDataHelper
{
    public class TableViewModel : Rhino.UI.ViewModel, INotifyPropertyChanged
    {
        private TableAttributes _tableAttributes;
  
        public CollectionViewSource TableViewSource { get; set; }

        public TableAttributes TableAttributes
        {
            get
            {
               return this._tableAttributes;
            }
            set
            {
                                this._tableAttributes = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Layer> _layers = new ObservableCollection<Layer>();

        public ObservableCollection<Layer> Layers
        {
            get => _layers;
            set
            {
                _layers = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateAttributesCommand { get; private set; }

        public TableViewModel(uint documentSerialNumber)
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
            
            this.Message = "View Model Has Loaded";


            var attributesCollection = TableAttributes.GetAllObjectsAttributes(doc);
            TableViewSource = new CollectionViewSource { Source = attributesCollection };

            UpdateAttributesCommand = new RelayCommand(param => UpdateAttributes(param));


        }

        public void Dispose()
        {
            RhinoDoc.LayerTableEvent -= OnLayerTableEvent;
        }

        public void UpdateAttributes(object parameter)
        {
            TableViewSource.Source = TableAttributes.GetAllObjectsAttributes(RhinoDoc.ActiveDoc);
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
            Layers.Clear();
            foreach (var layer in RhinoDoc.ActiveDoc.Layers)
            {
                Layers.Add(layer);
            }
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