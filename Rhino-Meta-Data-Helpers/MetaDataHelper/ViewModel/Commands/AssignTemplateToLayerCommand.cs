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
            return true;
        }

        public void Execute(object parameter)
        {

            var targetLayer = (Layer)parameter;

            // Get the current Rhino document
            var doc = Rhino.RhinoDoc.ActiveDoc;

            if (doc == null) return;

            var layerObjects = doc.Objects.FindByLayer(targetLayer.Name);

            // Create a list to hold the IDs of all selected objects
            List<Guid> selectedObjectIds = new List<Guid>();

            // Loop through all objects
            foreach (var rhinoObject in layerObjects)
            {
                // Check if the object is selected
                selectedObjectIds.Add(rhinoObject.Id);
            }

            // Loop through each UserStringDefinition in the current template
            foreach (var userStringDefinition in this._currentTemplate.Collection)
            {
                // Loop through each selected object ID
                foreach (var objectId in selectedObjectIds)
                {
                    // Assign the user string to the object
                    userStringDefinition.Assign(objectId);
                }
            }

            // Redraw the document to update the changes
            doc.Views.Redraw();
        }

        public event EventHandler CanExecuteChanged;
        public AssignTemplateToLayerCommand(UserStringTemplate currentTemplate)
        {
            _currentTemplate = currentTemplate;
        }
    }
}
