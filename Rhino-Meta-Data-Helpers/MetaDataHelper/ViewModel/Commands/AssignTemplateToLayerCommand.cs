using System;
using System.Collections.Generic;
using System.Windows.Input;
using Rhino.DocObjects;

namespace MetaDataHelper
{
    internal class AssignTemplateToLayerCommand : ICommand
    {
        private readonly UserStringTemplate _currentTemplate;

        public bool CanExecute(object parameter)
        {
            return parameter is Layer && _currentTemplate != null && _currentTemplate.Collection != null;
        }

        public void Execute(object parameter)
        {
            var targetLayer = parameter as Layer;
            if (targetLayer == null) return;

            var doc = Rhino.RhinoDoc.ActiveDoc;
            if (doc == null) return;

            var layerObjects = doc.Objects.FindByLayer(targetLayer);
            if (layerObjects == null) return;

            var selectedObjectIds = new List<Guid>();
            foreach (var rhinoObject in layerObjects)
            {
                selectedObjectIds.Add(rhinoObject.Id);
            }

            foreach (var userStringDefinition in this._currentTemplate.Collection)
            {
                foreach (var objectId in selectedObjectIds)
                {
                    userStringDefinition.Assign(objectId);
                }
            }

            doc.Views.Redraw();
        }

        public event EventHandler CanExecuteChanged;
        public AssignTemplateToLayerCommand(UserStringTemplate currentTemplate)
        {
            _currentTemplate = currentTemplate;
        }
    }
}
