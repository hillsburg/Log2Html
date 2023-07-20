using Log2Html.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Log2Html.ViewModel
{
    public class MainViewVM : INotifyPropertyChanged
    {
        public ICommand RemoveSettingCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<ColorSetting> _colorSettings = new List<ColorSetting>();
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<ColorSetting> ColorSettings { get; set; } = new ObservableCollection<ColorSetting>();

        public MainViewVM()
        {
            ColorSettings = new ObservableCollection<ColorSetting>();
            RemoveSettingCommand = new RelayCommand(RemoveSettingItem);
        }

        public void RemoveSettingItem(object guid)
        {
            var id = (string)guid;
            var item = ColorSettings.First(x => x.SettingId == id);
            ColorSettings.Remove(item);
        }

        public void RestoreSettings(string configFilePath)
        {
            var json = File.ReadAllText(configFilePath);
            if (string.IsNullOrEmpty(json.Replace("\r", "").Replace("\n", "")))
            {
                return;
            }
            _colorSettings = JsonConvert.DeserializeObject<List<ColorSetting>>(json);
            foreach (var item in _colorSettings)
            {
                ColorSettings.Add(item);
            }
        }

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
    }
}
