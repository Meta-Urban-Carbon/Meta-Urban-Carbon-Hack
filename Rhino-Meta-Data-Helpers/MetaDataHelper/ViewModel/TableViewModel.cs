using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System;
using System.Collections.Generic;
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
        private CollectionViewSource _collectionViewSource;

        public CollectionViewSource CollectionViewSource
        {
            get => _collectionViewSource;
            set
            {
                _collectionViewSource = value;
                OnPropertyChanged(nameof(CollectionViewSource));
            }
        }

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

        public event Action<IEnumerable<string>> ColumnsUpdated;

        public ICommand UpdateAttributesCommand { get; private set; }
        public ICommand SelectRhinoObjectCommand { get; private set; }


        public TableViewModel(uint documentSerialNumber)
        {
            this.DocumentRuntimeSerialNumber = documentSerialNumber;
            MetaDataHelperPlugin.Instance.SettingsSaved +=
                (sender, args) => SettingsChangedMessage = $"SettingSaved {++m_saved_count}";
            m_use_multiple_counters = this.UseMultipleCounters == true;
            Rhino.UI.Panels.Show += OnShowPanel;

            RhinoDoc.LayerTableEvent += OnLayerTableEvent;
            RhinoDoc.NewDocument += OnDocumentChange;
            RhinoDoc.BeginOpenDocument += OnDocumentChange;
            RhinoDoc.ActiveDocumentChanged += OnDocumentChange;
            RhinoDoc.EndOpenDocument += NewDocumentOpened;

            UpdateLayers();

            // Set the selected layer to the current layer in the active Rhino document
            var doc = Rhino.RhinoDoc.ActiveDoc;
            var currentLayerName = doc.Layers.CurrentLayer.Name;
            
            this.Message = "View Model Has Loaded";

            TableAttributes = new TableAttributes();
            CollectionViewSource = new CollectionViewSource
            {
                Source = TableAttributes
            };


            UpdateAttributesCommand = new RelayCommand(param => UpdateAttributes(param));
            SelectRhinoObjectCommand = new SelectRhinoObjectCommand();
            
        }

        public void Dispose()
        {
            RhinoDoc.LayerTableEvent -= OnLayerTableEvent;
        }

        /// <summary>
        /// On Document Change method clears all ref to layers
        /// </summary>
        /// <param name="parameter"></param>
        public void OnDocumentChange(object sender, DocumentEventArgs e)
        {
            Layers.Clear();
        }

        /// <summary>
        /// update layers when new document is opened
        ///
        /// </summary>
        public void NewDocumentOpened(object sender, DocumentOpenEventArgs e)
        {
            UpdateLayers();
        }

        public void UpdateAttributes(object parameter)
        {
            //Refresh the view
            this.TableAttributes.Refresh();

            // Raise the event with the updated keys
            ColumnsUpdated?.Invoke(TableAttributes.AllObjectKeys);
            //this.CollectionViewSource = new CollectionViewSource
            //{
            //    Source = TableAttributes
            //};
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

        private void SelectObject(object parameter)
        {
            if (parameter is Guid objectId)
            {
                // Implement the logic to select the Rhino object with this objectId
                // For example: RhinoDoc.ActiveDoc.Objects.Select(objectId);
            }
        }

        private void UpdateLayers()
        {
            var doc = RhinoDoc.ActiveDoc;
            var docLayers = doc.Layers;
            Layers.Clear();
            foreach (var layer in docLayers)
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