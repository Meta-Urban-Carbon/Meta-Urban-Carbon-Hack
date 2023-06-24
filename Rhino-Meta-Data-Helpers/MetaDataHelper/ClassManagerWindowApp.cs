using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Rhino;
using Rhino.Commands;
using RhinoWindows;

namespace MetaDataHelper
{
    class ClassManagerWindowApp
    {
        public ClassManagerWindowApp()
        {
            //Any inital Setup

        }

        public Result Run()
        {
            try
            {
                //Create Main WindowViewModel
                var viewModel = new ViewModel();

                //Create Main Window(WPF) with mainViewModel
                var mainWindow = new MainWindow(viewModel);

                //Create Helper to attach Rhino window(IWin32Window) as owner of Main Window(wpf)
                WindowInteropHelper helper = new WindowInteropHelper(mainWindow);
                helper.Owner = RhinoWinApp.MainWindow.Handle;

                //Open Window
                mainWindow.Show();

                RhinoApp.WriteLine("Class Manager Window Opened");
                return Result.Success;
            }
            catch
            {
                RhinoApp.WriteLine("Class Manager Failed to Launch");

                return Result.Failure;
            }
        }
    }
}
