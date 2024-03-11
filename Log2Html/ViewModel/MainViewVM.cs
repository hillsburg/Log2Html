using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Log2Html.Dao;
using Log2Html.Dao.Model;
using Log2Html.Model;
using Newtonsoft.Json;

namespace Log2Html.ViewModel
{
    public class MainViewVM : INotifyPropertyChanged
    {
        private DbUtils _dbHelper;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _currentConvertedFilePath;

        /// <summary>
        /// Key word
        /// </summary>
        public string CurrentConvertedFilePath
        {
            get => _currentConvertedFilePath;
            set
            {
                _currentConvertedFilePath = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand RemoveSettingCommand { get; set; }

        public ICommand RemoveConvertEntryCommand { get; set; }

        public ICommand FileAliasChangesCommand { get; set; }

        public ICommand OpenHtmlCommand { get; set; }

        public ICommand MenuAboutCommand { get; set; }



        private List<ColorSettingItem> _colorSettings = new List<ColorSettingItem>();

        public ObservableCollection<ColorSettingItem> ColorSettings { get; set; } = new ObservableCollection<ColorSettingItem>();
        public ObservableCollection<ConvertEntry> ConvertEntries { get; set; } = new ObservableCollection<ConvertEntry>();
        public MainViewVM()
        {
            ColorSettings = new ObservableCollection<ColorSettingItem>();
            RemoveSettingCommand = new RelayCommand(RemoveSettingItem);
            RemoveConvertEntryCommand = new RelayCommand(RemoveConvertedEntryItem);
            FileAliasChangesCommand = new RelayCommand(FileAliasChanged);
            OpenHtmlCommand = new RelayCommand(OpenFile);
            MenuAboutCommand = new RelayCommand(ShowAbout);
            _dbHelper = new DbUtils();
            _dbHelper.Init("Data Source=Log2Html.db");
            var convertEntries = _dbHelper.DbHelper.DbOperation.Query<ConvertEntryEntity>(null);
            convertEntries = convertEntries.OrderByDescending(x => x.ConvertDate).ToList();
            foreach (var item in convertEntries)
            {
                ConvertEntries.Add(new ConvertEntry()
                {
                    Id = item.Id,
                    FileNameAlias = item.FileNameAlias,
                    OriginalFilePath = item.OriginalFilePath,
                    ConvertedFilePath = item.ConvertedFilePath,
                    ConvertDate = item.ConvertDate
                });
            }

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
        /// delete the item to which the guid equals
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveConvertedEntryItem(object guid)
        {
            var id = (string)guid;
            var item = ConvertEntries.First(x => x.Id == id);
            _dbHelper.DbHelper.DbOperation.Delete<ConvertEntryEntity>(new ConvertEntryEntity() { Id = id });
            ConvertEntries.Remove(item);
        }

        /// <summary>
        /// delete the item to which the guid equals
        /// </summary>
        /// <param name="guid"></param>
        public void FileAliasChanged(object guid)
        {
            var id = (string)guid;
            var item = ConvertEntries.First(x => x.Id == id);

            var dbModel = new ConvertEntryEntity()
            {
                Id = item.Id,
                OriginalFilePath = item.OriginalFilePath,
                FileNameAlias = item.FileNameAlias,
                ConvertedFilePath = item.ConvertedFilePath,
                ConvertDate = item.ConvertDate
            };

            _dbHelper.DbHelper.DbOperation.Update<ConvertEntryEntity>(dbModel);

            item.IsReadOnly = true;
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
        /// <param name="filePath">filePath</param>
        /// <param name="logInfo">logInfo</param>
        /// <param name="htmlFilePath">htmlFilePath</param>
        /// <returns></returns>
        public ReturnInfo ConvertFile(string filePath, out string logInfo, out string htmlFilePath)
        {
            logInfo = string.Empty;
            htmlFilePath = string.Empty;

            if (string.IsNullOrEmpty(filePath))
            {
                return new ReturnInfo(false);
            }

            var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".html";
            htmlFilePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + newFileName);
            CurrentConvertedFilePath = htmlFilePath;
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

                        // Note that html colors differ from WPF color. The alpha is the first byte in WPF while it is the last byte in css
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
                var id = Guid.NewGuid().ToString();
                var convertedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // DB Operation
                _dbHelper.DbHelper.DbOperation.Insert<ConvertEntryEntity>(new ConvertEntryEntity()
                {
                    Id = id,
                    OriginalFilePath = filePath,
                    FileNameAlias = Path.GetFileNameWithoutExtension(htmlFilePath),
                    ConvertedFilePath = htmlFilePath,
                    ConvertDate = convertedTime
                });

                // UI operation
                ConvertEntries.Insert(0, new ConvertEntry()
                {
                    Id = id,
                    FileNameAlias = Path.GetFileNameWithoutExtension(htmlFilePath),
                    OriginalFilePath = filePath,
                    ConvertedFilePath = htmlFilePath,
                    ConvertDate = convertedTime
                });

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
            ColorSettings.Add(new ColorSettingItem());
        }

        private void OpenFile(object filePath)
        {
            var path = (string)filePath;
            if (!string.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                {
                    // System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
            }
        }

        private void ShowAbout(object filePath)
        {
            About about = new About();
            about.Owner = App.Current.MainWindow;
            about.Show();
        }
    }
}
