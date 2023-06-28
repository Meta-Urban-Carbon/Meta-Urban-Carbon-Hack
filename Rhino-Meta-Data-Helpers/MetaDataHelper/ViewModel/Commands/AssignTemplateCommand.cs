using MetaDataHelper.UserStringClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MetaDataHelper
{
    internal class AssignTemplateCommand : ICommand
    {
        private UserStringTemplate _currentTemplate;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //Cast to Assign Target Type or Default to Selected. 
            var target = parameter is AssignTargetEnum ? (AssignTargetEnum)parameter : AssignTargetEnum.Selected;

            // Get the current Rhino document
            var doc = Rhino.RhinoDoc.ActiveDoc;

            if (doc == null) return;

            // Get all objects in the document
            var allObjects = doc.Objects;

            // Create a list to hold the IDs of all selected objects
            List<Guid> selectedObjectIds = new List<Guid>();

            // Loop through all objects
            foreach (var rhinoObject in allObjects)
            {
                // Check if the object is selected
                if (rhinoObject.IsSelected(false) > 0)
                {
                    // If it is, add its ID to the list
                    selectedObjectIds.Add(rhinoObject.Id);
                }
            }

            // Loop through each UserStringDefinition in the current template
            foreach (var userStringDefinition in this._currentTemplate)
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

        public AssignTemplateCommand(UserStringTemplate currentTemplate)
        {
            _currentTemplate = currentTemplate;
        }
    }
}
