using System;
using System.Windows;
using System.Windows.Controls;

namespace Log2Html
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _configFilePath = "settings.json";

        public MainWindow()
        {
            InitializeComponent();
            Vm.RestoreSettings(_configFilePath);
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var info = Vm.ConvertFile(files[0], out var logInfo, out var htmlFilePath);
            }
        }

        private void Save_Settings(object sender, EventArgs e)
        {
            Vm.SaveSettings(_configFilePath);
        }

        private void Add_Setting(object sender, EventArgs e)
        {
            Vm.AddSettingItem();
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Vm.SaveSettings(_configFilePath);
        }

        private void FileNameAlias_TextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = false;
            }
        }
    }
}

