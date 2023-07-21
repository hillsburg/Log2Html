using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Log2Html.Model
{
    public class ColorSettingItem : INotifyPropertyChanged
    {
        private string _settingId = System.Guid.NewGuid().ToString("N");

        private string _key;

        private string _colorRgb = "#00000000";

        private bool _shouldApplyForAllLine = true;

        private bool _shouldIgnoreCase = false;

        /// <summary>
        /// ID of setting item
        /// </summary>
        public string SettingId { get => _settingId; }

        /// <summary>
        /// Key word
        /// </summary>
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Color of the key word
        /// </summary>
        public string ColorRgb
        {
            get => _colorRgb;
            set
            {
                _colorRgb = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Should apply for the whole line
        /// </summary>
        public bool ShouldApplyForAllLine
        {
            get => _shouldApplyForAllLine;
            set
            {
                _shouldApplyForAllLine = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Should ignore key word case
        /// </summary>
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
