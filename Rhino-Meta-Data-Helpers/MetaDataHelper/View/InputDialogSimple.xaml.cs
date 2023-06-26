using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MetaDataHelper
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialogSimple : Window
    {
        public string ResponseText
        {
            get { return InputTextBox.Text; }
            set { InputTextBox.Text = value; }
        }

        public InputDialogSimple (string question)
        {
            InitializeComponent();
            Title = question;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
