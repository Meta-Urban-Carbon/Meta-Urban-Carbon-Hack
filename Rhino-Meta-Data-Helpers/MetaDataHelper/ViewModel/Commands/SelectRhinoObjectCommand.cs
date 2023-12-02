using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Input;
using Grasshopper.GUI.Canvas;
using Rhino;
using Rhino.DocObjects;

namespace MetaDataHelper
{
    internal class SelectRhinoObjectCommand: ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
            //throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            if (parameter is Guid objectId)
            {
                var _doc = Rhino.RhinoDoc.ActiveDoc; // Get the active Rhino document

                // Deselect all objects first, if you want only this object to be selected
                _doc.Objects.UnselectAll();

                // Find the Rhino object by its GUID
                var rhinoObject = _doc.Objects.Find(objectId);
                if (rhinoObject != null)
                {
                    // Select the Rhino object
                    rhinoObject.Select(true);

                    // Redraw the document to update the view
                    _doc.Views.Redraw();
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        public SelectRhinoObjectCommand()   
        {
        }

    }
}

