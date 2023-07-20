using System;
using System.Collections.Generic;
using System.IO;
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
                ConvertFile(files[0]);
            }
        }

        private void ConvertFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
            txt_log.Text = "";
            var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".html";
            var htmlPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + newFileName);
            try
            {
                var lines = File.ReadAllLines(filePath);
                var newLines = new List<string>();
                newLines.Add("<html><head><style>p{margin: 3px 3px 3px 3px;}</style></head><body>");
                foreach (var line in lines)
                {
                    var tempLine = line;
                    foreach (var item in vm.ColorSettings)
                    {
                        if (string.IsNullOrEmpty(item.Key) || string.IsNullOrWhiteSpace(item.Key))
                        {
                            continue;
                        }
                        // Note that html color differs WPF color. The alpha is the first byte while css is the last byte
                        var htmlCssColor = item.ColorRgb.StartsWith("#") && item.ColorRgb.Length > 8 ? "#" + item.ColorRgb.Substring(3) + item.ColorRgb.Substring(1, 2) : item.ColorRgb;
                        if (tempLine.Contains(item.Key))
                        {
                            if (item.ShouldApplyForAllLine)
                            {
                                if (tempLine.StartsWith("<p"))
                                {
                                    var substr = tempLine.Substring(0, tempLine.IndexOf(">") + 1);
                                    tempLine = tempLine.Replace(substr, $"<p style=\"color:{htmlCssColor};\">");
                                }
                                else
                                {
                                    tempLine = $"<p style=\"color: {htmlCssColor};\">{tempLine}</p>";
                                }
                            }
                            else
                            {
                                tempLine = $"<p>{tempLine.Replace(item.Key, $"<span style=\"color: {htmlCssColor};\">{item.Key}</span>")}</p>";
                            }
                        }
                    }
                    newLines.Add(tempLine);
                }
                newLines.Add($"</body></html>");

                using (var writer = new StreamWriter(htmlPath))
                {
                    foreach (var item in newLines)
                    {
                        writer.WriteLine(item);
                    }
                }
                txt_html_path.Text = htmlPath;
                txt_log.Text = "Perfect!";

            }
            catch (Exception ex)
            {
                txt_log.Text = $"哦豁！不得行！你看：{ex.Message}";
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
            vm.ColorSettings.Add(new Model.ColorSetting()
            {
                ColorRgb = "",
                Key = "",
                ShouldApplyForAllLine = true,
            });
        }
        //private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        //{
        //    TextBox.Text = "#" + ClrPcker_Background.SelectedColor.R.ToString() + ClrPcker_Background.SelectedColor.G.ToString() + ClrPcker_Background.SelectedColor.B.ToString();
        //}

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vm.SaveSettings(_configFilePath);
        }
    }
}

