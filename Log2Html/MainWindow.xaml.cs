using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            vm.RestoreSettings(_configFilePath);
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var info = vm.ConvertFile(files[0], out var logInfo, out var htmlFilePath);
                txt_log.Text = logInfo;
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            Message message = new Message();
            message.Show();
        }

        private void Save_Settings(object sender, EventArgs e)
        {
            vm.SaveSettings(_configFilePath);
        }

        private void Add_Setting(object sender, EventArgs e)
        {
            vm.AddSettingItem();
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.SaveSettings(_configFilePath);
        }

        private void FileNameAlias_TextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = false;
            }
        }

        private void FileNameAliasLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.IsReadOnly = true;
            }
        }

        private void FilaNameAliasKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                if (e.Key == Key.Enter)
                {
                    textBox.IsReadOnly = true;
                }
            }
        }
    }
}

