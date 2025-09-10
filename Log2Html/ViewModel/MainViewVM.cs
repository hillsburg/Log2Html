using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Log2Html.Dao;
using Log2Html.Dao.Model;
using Log2Html.Enum;
using Log2Html.Model;
using Log2Html.Utils;
using Newtonsoft.Json;

namespace Log2Html.ViewModel
{
    public class MainViewVM : INotifyPropertyChanged
    {
        private DbUtils _dbHelper;

        public event PropertyChangedEventHandler? PropertyChanged;

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

        public ICommand RemoveColorSettingItemCommand { get; set; }

        public ICommand RemoveConvertedEntryCommand { get; set; }

        public ICommand ConvertedFileAliasChangesCommand { get; set; }

        public ICommand OpenHtmlInBrowserCommand { get; set; }

        public ICommand MenuAboutCommand { get; set; }

        public ICommand PasteCommand { get; set; }

        public ICommand EntryItemDeleteCommand { get; set; }

        public ICommand EntryItemOpenOriginalFileCommand { get; set; }

        public ObservableCollection<LogItem> LogItemList { get; set; } = new();

        public void AddLog(LogItem logItem)
        {
            LogItemList.Add(logItem);
        }

        public void ClearLog()
        {
            LogItemList.Clear();
        }

        private List<ColorSettingItem> _colorSettings = new List<ColorSettingItem>();

        /// <summary>
        /// Color settings items
        /// </summary>
        public ObservableCollection<ColorSettingItem> ColorSettings { get; set; } = new ObservableCollection<ColorSettingItem>();

        /// <summary>
        /// Converted entries
        /// </summary>
        public ObservableCollection<ConvertEntry> ConvertEntries { get; set; } = new ObservableCollection<ConvertEntry>();

        /// <summary>
        /// ctor
        /// </summary>
        public MainViewVM()
        {
            ColorSettings = new ObservableCollection<ColorSettingItem>();
            RemoveColorSettingItemCommand = new RelayCommand(RemoveSettingItem);
            RemoveConvertedEntryCommand = new RelayCommand(RemoveConvertedEntryItem);
            ConvertedFileAliasChangesCommand = new RelayCommand(FileAliasChanged);
            OpenHtmlInBrowserCommand = new RelayCommand(OpenFileInBrowser);
            EntryItemOpenOriginalFileCommand = new RelayCommand(OpenFileInFileExplorer);
            EntryItemDeleteCommand = new RelayCommand(DeleteEntryItem);
            MenuAboutCommand = new RelayCommand(ShowAbout);
            PasteCommand = new RelayCommand(PasteRgb);

            _dbHelper = new DbUtils("Data Source=Log2Html.db", SqlSugar.DbType.Sqlite);

            var convertEntries = _dbHelper.Db.Queryable<ConvertEntryEntity>().ToList();
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
            if (guid == null)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure to delete this item?", "Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var id = (string)guid;
                var item = ConvertEntries.First(x => x.Id == id);
                if (item == null)
                {
                    return;
                }

                _dbHelper.Db.Deleteable(new ConvertEntryEntity() { Id = id });
                ConvertEntries.Remove(item);
            }
        }

        /// <summary>
        /// delete the item to which the guid equals
        /// </summary>
        /// <param name="guid"></param>
        public void FileAliasChanged(object guid)
        {
            if (guid == null)
            {
                return;
            }

            var id = (string)guid;
            var item = ConvertEntries.First(x => x.Id == id);
            if (item == null)
            {
                return;
            }

            var dbModel = new ConvertEntryEntity()
            {
                Id = item.Id,
                OriginalFilePath = item.OriginalFilePath,
                FileNameAlias = item.FileNameAlias,
                ConvertedFilePath = item.ConvertedFilePath,
                ConvertDate = item.ConvertDate
            };

            _dbHelper.Db.Updateable<ConvertEntryEntity>(dbModel);

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

            if (ColorSettings.Count == 0)
            {
                AddLog(LogLevel.Error, "Color setting is empty", LogDestination.DispalyAndLogFile);
                return new ReturnInfo(false);
            }

            if (ColorSettings.All(x => string.IsNullOrEmpty(x.Key)))
            {
                AddLog(LogLevel.Error, "Key word of color setting is empty", LogDestination.DispalyAndLogFile);
                return new ReturnInfo(false);
            }

            var newFileName = DateTime.Now.ToString("_yyyyMMddHHmmss") + ".html";
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

                    // replace < and > to avoid html tag
                    tempLine = tempLine.Replace("<", "&lt;");
                    tempLine = tempLine.Replace(">", "&gt;");
                    bool isMatched = false;
                    foreach (var item in ColorSettings)
                    {
                        if (string.IsNullOrEmpty(item.Key) || string.IsNullOrWhiteSpace(item.Key))
                        {
                            continue;
                        }

                        // replace < and > to avoid html tag
                        var key = item.Key.Replace("<", "&lt;");
                        key = key.Replace(">", "&gt;");

                        // Note that html colors differ from WPF color. The alpha is the first byte in WPF while it is the last byte in css
                        var htmlCssColor = item.ColorRgb.StartsWith("#") && item.ColorRgb.Length > 8 ? "#" + item.ColorRgb.Substring(3) + item.ColorRgb.Substring(1, 2) : item.ColorRgb;
                        if (tempLine.Contains(key))
                        {
                            if (!isMatched)
                            {
                                isMatched = true;
                            }

                            if (item.ShouldApplyForAllLine)
                            {
                                if (tempLine.StartsWith("<p"))
                                {
                                    // remove the style of old p tag
                                    var substr = tempLine.Substring(0, tempLine.IndexOf(">") + 1);

                                    // add new style
                                    tempLine = tempLine.Replace(substr, $"<p style=\"color:{htmlCssColor};\">");
                                }
                                else
                                {
                                    tempLine = $"<p style=\"color: {htmlCssColor};\">{tempLine}</p>";
                                }
                            }
                            else
                            {
                                tempLine = $"<p>{tempLine.Replace(key, $"<span style=\"color: {htmlCssColor};\">{key}</span>")}</p>";
                            }
                        }
                    }

                    // if no key word is matched, add <p> tag
                    if (!isMatched)
                    {
                        // strings are immutable, all replace operations above do not change the original string line
                        tempLine = $"<p>{line}</p>";
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
                AddLog(LogLevel.Info, logInfo, LogDestination.DispalyAndLogFile);

                var id = Guid.NewGuid().ToString();
                var convertedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var fileName = Path.GetFileNameWithoutExtension(htmlFilePath);
                // DB Operation
                _dbHelper.Db.Insertable(new ConvertEntryEntity()
                {
                    Id = id,
                    OriginalFilePath = filePath,
                    FileNameAlias = fileName.Substring(0, fileName.LastIndexOf('_')),
                    ConvertedFilePath = htmlFilePath,
                    ConvertDate = convertedTime
                });

                // UI operation
                ConvertEntries.Insert(0, new ConvertEntry()
                {
                    Id = id,
                    FileNameAlias = fileName.Substring(0, fileName.LastIndexOf('_')),
                    OriginalFilePath = filePath,
                    ConvertedFilePath = htmlFilePath,
                    ConvertDate = convertedTime
                });

                return new ReturnInfo(true);
            }
            catch (Exception ex)
            {
                logInfo = $"哦豁！不得行！你看：{ex.Message}";
                AddLog(LogLevel.Error, logInfo, LogDestination.DispalyAndLogFile);
                return new ReturnInfo(false, ex.Message, 0);
            }
        }

        public void AddSettingItem()
        {
            ColorSettings.Add(new ColorSettingItem());
        }

        private void OpenFileInBrowser(object filePath)
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

        private void OpenFileInFileExplorer(object filePath)
        {
            var path = (string)filePath;
            if (string.IsNullOrEmpty(path)) return;
            if (File.Exists(path))
            {
                Process.Start("explorer.exe", "/select," + filePath);
            }
        }

        private void DeleteEntryItem(object id)
        {
            if (id == null)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure to delete this item?", "Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var guid = (string)id;
                ConvertEntries.Remove(ConvertEntries.First(x => x.Id == guid));
            }
        }

        private void ShowAbout(object filePath)
        {
            About about = new About();
            about.Owner = App.Current.MainWindow;
            about.Show();
        }

        private void PasteRgb(object settingItemId)
        {
            if (settingItemId == null)
            {
                return;
            }

            var rgbRegEx = new Regex(@"^#([0-9a-fA-F]{6}|[0-9a-fA-F]{8})$");
            var rgbStr = Clipboard.GetText();
            if (string.IsNullOrEmpty(rgbStr))
            {
                AddLog(LogLevel.Error, $"Empty rgb. Check your clipboard", LogDestination.DispalyAndLogFile);
                return;
            }

            if (!rgbRegEx.Match(rgbStr).Success)
            {
                AddLog(LogLevel.Error, $"\"{rgbStr}\" is not in the correct rgb format", LogDestination.DispalyAndLogFile);
                return;
            }

            var guid = (string)settingItemId;
            ColorSettings.First(x => x.SettingId == guid).ColorRgb = rgbStr;
        }

        private void AddLog(LogLevel logLevel, string message, LogDestination destination)
        {
            switch (destination)
            {
                case LogDestination.DispalyAndLogFile:
                    AddLog(new LogItem()
                    {
                        TimeStamp = DateTime.Now.ToString(),
                        LogLevel = logLevel,
                        LogContent = message,
                        Destination = destination
                    });
                    LogHelper.AddLog(logLevel, message);
                    break;

                case LogDestination.OnlyLogFile:
                    LogHelper.AddLog(logLevel, message);
                    break;

                case LogDestination.OnlyDispaly:
                    AddLog(new LogItem()
                    {
                        TimeStamp = DateTime.Now.ToShortTimeString(),
                        LogLevel = logLevel,
                        LogContent = message,
                        Destination = destination
                    });
                    break;
                default:
                    throw new ArgumentException("Unknown log destination");
            }

        }
    }
}
