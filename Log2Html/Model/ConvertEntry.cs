using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Log2Html.Model
{
    public class ConvertEntry : INotifyPropertyChanged
    {
        private string _id;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isReadOnly = true;

        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                NotifyPropertyChanged();
            }
        }

        private string _fileNameAlias;

        public string FileNameAlias
        {
            get => _fileNameAlias;
            set
            {
                _fileNameAlias = value;
                NotifyPropertyChanged();
            }
        }

        private string _originalFilePath;

        public string OriginalFilePath
        {
            get => _originalFilePath;
            set
            {
                _originalFilePath = value;
                NotifyPropertyChanged();
            }
        }

        private string _convertedFilePath;

        public string ConvertedFilePath
        {
            get => _convertedFilePath;
            set
            {
                _convertedFilePath = value;
                NotifyPropertyChanged();
            }
        }

        private string _convertDate;

        public string ConvertDate
        {
            get => _convertDate;
            set
            {
                _convertDate = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
