using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Log2Html.Model
{
    public class ColorSetting : INotifyPropertyChanged
    {
        private string _settingId = System.Guid.NewGuid().ToString("N");

        private string _key;

        private string _colorRgb = "#00000000";

        private bool _shouldApplyForAllLine = true;

        private bool _shouldIgnoreCase = false;
        public string SettingId { get => _settingId; }
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                NotifyPropertyChanged();
            }
        }
        public string ColorRgb
        {
            get => _colorRgb;
            set
            {
                _colorRgb = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShouldApplyForAllLine
        {
            get => _shouldApplyForAllLine;
            set
            {
                _shouldApplyForAllLine = value;
                NotifyPropertyChanged();
            }
        }
        public bool ShouldIgnoreCase
        {
            get => _shouldIgnoreCase;
            set
            {
                _shouldIgnoreCase = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
