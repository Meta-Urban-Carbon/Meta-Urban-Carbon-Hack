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
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;

namespace MetaDataHelper
{
    public class TableViewModel : Rhino.UI.ViewModel, INotifyPropertyChanged
    {
        private TableAttributes _tableAttributes;
        private CollectionViewSource _collectionViewSource;
        private readonly DispatcherTimer _filterRefreshTimer;

        // Prevent re-entrant refreshes during initialization/bulk updates
        private bool _suppressRefresh;

        // Monitor selection changes when "Selected only" is active
        private readonly DispatcherTimer _selectionMonitorTimer;
        private int _lastSelectedCount = -1;

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
            get { return this._tableAttributes; }
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

        // New: filter options for layers including an explicit "All layers"
        public class LayerFilterOption
        {
            public string Name { get; set; }
            public Layer Layer { get; set; }
        }

        private ObservableCollection<LayerFilterOption> _layerFilterOptions = new ObservableCollection<LayerFilterOption>();
        public ObservableCollection<LayerFilterOption> LayerFilterOptions
        {
            get => _layerFilterOptions;
            set
            {
                _layerFilterOptions = value;
                OnPropertyChanged();
            }
        }

        private LayerFilterOption _selectedLayerFilterOption;
        public LayerFilterOption SelectedLayerFilterOption
        {
            get => _selectedLayerFilterOption;
            set
            {
                _selectedLayerFilterOption = value;
                // Map to underlying FilterLayer used by filtering
                FilterLayer = value?.Layer; // this will call ScheduleFilterRefresh
                OnPropertyChanged();
            }
        }

        // Filters and live edit
        private bool _liveEdit = true;
        public bool LiveEdit { get => _liveEdit; set { _liveEdit = value; OnPropertyChanged(); } }

        // Default to false so the grid shows items at startup
        private bool _filterSelectedOnly = false;
        public bool FilterSelectedOnly
        {
            get => _filterSelectedOnly;
            set
            {
                _filterSelectedOnly = value;
                OnPropertyChanged();
                // Start/stop monitoring selection changes based on this toggle
                if (_filterSelectedOnly)
                    StartSelectionMonitor();
                else
                    StopSelectionMonitor();
                // Immediately update on filter change
                ScheduleFilterRefresh();
            }
        }

        // Default visible filter can hide all rows if nothing is visible; start with false for clarity
        private bool _filterVisibleOnly = false;
        public bool FilterVisibleOnly
        {
            get => _filterVisibleOnly;
            set
            {
                _filterVisibleOnly = value;
                OnPropertyChanged();
                // Immediately update on filter change
                ScheduleFilterRefresh();
            }
        }

        private Layer _filterLayer;
        public Layer FilterLayer
        {
            get => _filterLayer;
            set
            {
                _filterLayer = value;
                OnPropertyChanged();
                // Immediately update on filter change
                ScheduleFilterRefresh();
            }
        }

        public event Action<IEnumerable<string>> ColumnsUpdated;

        // Helper to raise the event from external command classes
        public void RaiseColumnsUpdated()
        {
            ColumnsUpdated?.Invoke(TableAttributes?.AllObjectKeys ?? new List<string>());
        }

        public ICommand SelectRhinoObjectCommand { get; private set; }
        public ICommand ExportCsvCommand { get; private set; }

        // Cache to decide if selection filter should be applied
        private bool _hasSelection;

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

            this.Message = "View Model Has Loaded";

            // Initialize collections BEFORE any refresh can be triggered
            TableAttributes = new TableAttributes();
            CollectionViewSource = new CollectionViewSource { Source = TableAttributes };

            // Suppress refresh during initial layer population which sets FilterLayer via SelectedLayerFilterOption
            _suppressRefresh = true;
            UpdateLayers();

            // Throttle timer for filter changes (kept for future use)
            _filterRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _filterRefreshTimer.Tick += (s, e) =>
            {
                _filterRefreshTimer.Stop();
                UpdateAttributes(null);
            };

            // Selection monitor timer
            _selectionMonitorTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
            _selectionMonitorTimer.Tick += (s, e) => OnSelectionMonitorTick();

            SelectRhinoObjectCommand = new SelectRhinoObjectCommand();
            ExportCsvCommand = new ExportCsvCommand(this);

            // End suppression and perform initial populate
            _suppressRefresh = false;
            UpdateAttributes(null);
        }

        private void StartSelectionMonitor()
        {
            _lastSelectedCount = -1; // force initial update
            if (!_selectionMonitorTimer.IsEnabled)
                _selectionMonitorTimer.Start();
        }

        private void StopSelectionMonitor()
        {
            if (_selectionMonitorTimer.IsEnabled)
                _selectionMonitorTimer.Stop();
            _lastSelectedCount = -1;
        }

        private void OnSelectionMonitorTick()
        {
            if (!FilterSelectedOnly)
                return;
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null)
                return;
            // count selected objects; exclude lights and grips
            var selected = doc.Objects?.GetSelectedObjects(false, false);
            int count = selected?.Count() ?? 0; // FIX: Use Count() instead of Length
            if (count != _lastSelectedCount)
            {
                _lastSelectedCount = count;
                UpdateAttributes(null);
            }
        }

        private void ScheduleFilterRefresh()
        {
            // React immediately to filter changes to keep UI in sync, unless suppressed
            if (_suppressRefresh)
                return;
            UpdateAttributes(null);
        }

        public void Dispose()
        {
            RhinoDoc.LayerTableEvent -= OnLayerTableEvent;
            RhinoDoc.NewDocument -= OnDocumentChange;
            RhinoDoc.BeginOpenDocument -= OnDocumentChange;
            RhinoDoc.ActiveDocumentChanged -= OnDocumentChange;
            RhinoDoc.EndOpenDocument -= NewDocumentOpened;
            Rhino.UI.Panels.Show -= OnShowPanel;

            if (_filterRefreshTimer != null)
                _filterRefreshTimer.Stop();
            if (_selectionMonitorTimer != null)
                _selectionMonitorTimer.Stop();
        }

        public void UpdateAttributes(object parameter)
        {
            // Refresh from doc with filter
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null)
            {
                Message = "No active document";
                return;
            }

            // Ensure collection exists (defensive)
            if (TableAttributes == null)
            {
                TableAttributes = new TableAttributes();
                if (CollectionViewSource == null)
                    CollectionViewSource = new CollectionViewSource();
                CollectionViewSource.Source = TableAttributes;
            }

            // reset the keys for this snapshot so columns match visible data
            TableAttributes.AllObjectKeys = new List<string>();
            TableAttributes.Clear();

            var snapshot = GetDocumentObjectsSnapshot(doc);

            // Determine if there is any selection in the current snapshot
            _hasSelection = snapshot.Any(x => x != null && x.IsSelected(false) > 0);

            int total = 0, added = 0, selected = 0, visible = 0;
            foreach (var o in snapshot)
            {
                if (o == null) continue;
                total++;
                if (o.IsSelected(false) > 0) selected++;
                if (!o.IsHidden && o.Visible) visible++;

                if (!PassesFilters(o)) continue;
                TableAttributes.Add(o);
                added++;
            }

            Message = $"Objects: {total}, Selected: {selected}, Visible: {visible}, Added: {added}, Layer: {(FilterLayer != null ? FilterLayer.Name : "All layers")}, Filters => SelOnly:{FilterSelectedOnly}, VisOnly:{FilterVisibleOnly}";

            ColumnsUpdated?.Invoke(TableAttributes.AllObjectKeys);
        }

        private List<RhinoObject> GetDocumentObjectsSnapshot(RhinoDoc doc)
        {
            // Use a concrete array snapshot via object type filter to avoid null enumeration issues
            var arr = doc.Objects?.GetObjectList(ObjectType.AnyObject) ?? Array.Empty<RhinoObject>();

            if (FilterLayer != null)
                return arr.Where(o => o != null && o.Attributes != null && o.Attributes.LayerIndex == FilterLayer.Index).ToList();

            return arr.Where(o => o != null).ToList();
        }


        private bool PassesFilters(RhinoObject o)
        {
            if (o == null) return false;
            if (FilterSelectedOnly && _hasSelection && o.IsSelected(false) == 0) return false;
            if (FilterVisibleOnly && (o.IsHidden || !o.Visible)) return false;
            if (FilterLayer != null && o.Attributes.LayerIndex != FilterLayer.Index) return false;
            return true;
        }

        public void ApplyCellEdit(RhinoObjectTableAttributes row, string key, string newValue)
        {
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null) return;

            var obj = doc.Objects.FindId(row.ObjectGuid);
            if (obj == null) return;

            obj.Attributes.SetUserString(key, newValue ?? string.Empty);
            obj.CommitChanges();

            doc.Views.Redraw();
        }

        public void ApplySelectedRows(IList<RhinoObjectTableAttributes> rows)
        {
            var doc = RhinoDoc.ActiveDoc;
            if (doc == null || rows == null || rows.Count == 0) return;

            foreach (var row in rows)
            {
                var obj = doc.Objects.FindId(row.ObjectGuid);
                if (obj == null) continue;

                foreach (var kvp in row.Attributes)
                {
                    if (kvp.Key == "ObjectGuid") continue;
                    obj.Attributes.SetUserString(kvp.Key, kvp.Value ?? string.Empty);
                }
                obj.CommitChanges();
            }
            doc.Views.Redraw();
        }

        public void OnDocumentChange(object sender, DocumentEventArgs e)
        {
            if (_filterRefreshTimer.IsEnabled)
                _filterRefreshTimer.Stop();

            _suppressRefresh = true;

            Layers.Clear();
            LayerFilterOptions.Clear();
            TableAttributes?.Clear();

            _filterLayer = null;
            _selectedLayerFilterOption = null;
            OnPropertyChanged(nameof(FilterLayer));
            OnPropertyChanged(nameof(SelectedLayerFilterOption));

            RaiseColumnsUpdated();

            _suppressRefresh = false;
        }

        public void NewDocumentOpened(object sender, DocumentOpenEventArgs e)
        {
            UpdateLayers();
            UpdateAttributes(null);
        }

        private void OnLayerTableEvent(object sender, LayerTableEventArgs e)
        {
            if (e.EventType == LayerTableEventType.Added ||
                e.EventType == LayerTableEventType.Deleted ||
                e.EventType == LayerTableEventType.Modified)
            {
                UpdateLayers();
            }
        }

        private void UpdateLayers()
        {
            var doc = RhinoDoc.ActiveDoc;
            var docLayers = doc?.Layers;

            var previous = SelectedLayerFilterOption?.Name;

            Layers.Clear();
            LayerFilterOptions.Clear();

            LayerFilterOptions.Add(new LayerFilterOption { Name = "All layers", Layer = null });

            if (docLayers != null)
            {
                foreach (var layer in docLayers)
                {
                    Layers.Add(layer);
                    LayerFilterOptions.Add(new LayerFilterOption { Name = layer.Name, Layer = layer });
                }
            }

            SelectedLayerFilterOption = LayerFilterOptions.FirstOrDefault(o => o.Name == previous)
                                        ?? LayerFilterOptions.FirstOrDefault();
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}