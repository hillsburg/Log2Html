﻿using System;
using System.Windows;

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
                txt_html_path.Text = htmlFilePath;
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

        private void Open_Html_File(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(txt_html_path.Text);
            System.Diagnostics.Process.Start(@"D:\\CODE\\NEWest\\automotiveota\\code\\GTS.MaxSign_bin\\Result\\GPS L1\\004008001_20230724105829\\Test20230725110454.html");
        }
    }
}

