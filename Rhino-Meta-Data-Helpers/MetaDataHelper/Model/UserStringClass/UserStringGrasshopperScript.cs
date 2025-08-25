using System;
using System.Threading.Tasks;
using Rhino.UI; // Assuming this provides the Application.Invoke method or similar
using Grasshopper.Kernel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MetaDataHelper
{
    public class UserStringGrasshopperScript: INotifyPropertyChanged
    {
        private string _filePath = Settings.DefaultTemplatePath + "grasshopper_example.gh";

        public string FilePath {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }   

        public async Task RunFilesAsync() // This should be an async Task, not async void
        {
            var io = new GH_DocumentIO();
            var myServer = Grasshopper.Instances.DocumentServer;
            if (myServer == null) return;

            string filepath = this._filePath;
            if (!io.Open(filepath)) return; // You should check the return value of Open
            var ghDoc = io.Document;

            if (ghDoc == null) return;

            // If you need to perform some operations on a separate thread, you can do so like this:
            await Task.Run(() =>
            {
                // Perform any heavy computations here.
                // Be sure not to interact with the Grasshopper document or any other UI elements here.
            });

            // After running the task, you need to use Rhino's way of invoking on the main thread
            // because enabling a document and expiring its solution must be done on the main thread.
            // Application.Invoke is a common pattern in GUI frameworks to marshal calls back to the main thread.
            Rhino.RhinoApp.InvokeOnUiThread((Action)(() =>
            {
                // Now it is safe to interact with the Grasshopper document.
                ghDoc.Enabled = true;
                ghDoc.ExpireSolution();
                myServer.AddDocument(ghDoc);
                if (Grasshopper.Instances.ActiveCanvas != null)
                {
                    Grasshopper.Instances.ActiveCanvas.Document = ghDoc;
                }

                // Trigger the new solution
                try
                {
                    ghDoc.NewSolution(true, GH_SolutionMode.Silent);
                }
                catch
                {
                    // Log or handle exception as necessary
                }
            }));
        }

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
