using Log2Html.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Log2Html.ViewModel
{
    public class MainViewVM
    {
        public ICommand RemoveSettingCommand { get; set; }

        private List<ColorSettingItem> _colorSettings = new List<ColorSettingItem>();

        public ObservableCollection<ColorSettingItem> ColorSettings { get; set; } = new ObservableCollection<ColorSettingItem>();

        public MainViewVM()
        {
            ColorSettings = new ObservableCollection<ColorSettingItem>();
            RemoveSettingCommand = new RelayCommand(RemoveSettingItem);
        }

        /// <summary>
        /// delete the item to which the guid equals
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveSettingItem(object guid)
        {
            var id = (string)guid;
            var item = ColorSettings.First(x => x.SettingId == id);
            ColorSettings.Remove(item);
        }

        /// <summary>
        /// Restore config settings from json file
        /// </summary>
        /// <param name="configFilePath"></param>
        public void RestoreSettings(string configFilePath)
        {
            var json = File.ReadAllText(configFilePath);
            if (string.IsNullOrEmpty(json.Replace("\r", "").Replace("\n", "")))
            {
                return;
            }
            _colorSettings = JsonConvert.DeserializeObject<List<ColorSettingItem>>(json);
            foreach (var item in _colorSettings)
            {
                ColorSettings.Add(item);
            }
        }

        /// <summary>
        /// Save setting into json files
        /// </summary>
        /// <param name="configFilePath"></param>
        public void SaveSettings(string configFilePath)
        {
            _colorSettings.Clear();
            try
            {
                foreach (var item in ColorSettings)
                {
                    _colorSettings.Add(item);
                    var json = JsonConvert.SerializeObject(_colorSettings);
                    File.WriteAllText(configFilePath, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Convert txt file into Html
        /// </summary>
        /// <param name="filePath"></param>
        public ReturnInfo ConvertFile(string filePath, out string logInfo, out string htmlFilePath)
        {
            logInfo = string.Empty;
            htmlFilePath = string.Empty;

            if (string.IsNullOrEmpty(filePath))
                return new ReturnInfo(false);
            var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".html";
            htmlFilePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + newFileName);
            try
            {
                var lines = File.ReadAllLines(filePath);
                var newLines = new List<string>();
                newLines.Add("<html><head><style>p{margin: 3px 3px 3px 3px;}</style></head><body>");
                foreach (var line in lines)
                {
                    var tempLine = line;
                    foreach (var item in ColorSettings)
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

                using (var writer = new StreamWriter(htmlFilePath))
                {
                    foreach (var item in newLines)
                    {
                        writer.WriteLine(item);
                    }
                }
                logInfo = "Perfect!";
                return new ReturnInfo(true);
            }
            catch (Exception ex)
            {
                logInfo = $"哦豁！不得行！你看：{ex.Message}";
                return new ReturnInfo(false, ex.Message, 0);
            }
        }

        public void AddSettingItem()
        {
            ColorSettings.Add(new ColorSettingItem()
            {
                ColorRgb = "",
                Key = "",
                ShouldApplyForAllLine = true,
            });
        }
    }
}
