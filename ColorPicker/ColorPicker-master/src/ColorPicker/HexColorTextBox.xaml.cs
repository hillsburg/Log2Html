using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ColorPicker
{
    public partial class HexColorTextBox : PickerControlBase
    {
        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(HexColorTextBox),
                new PropertyMetadata(true));

        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        public HexColorTextBox() : base()
        {
            InitializeComponent();
        }

        private void ColorToHexConverter_OnShowAlphaChange(object sender, System.EventArgs e)
        {
            textbox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            //InvalidateProperty(SelectedColorProperty);
            //Color.RaisePropertyChanged(nameof(Color.RGB_R));
        }

        private void Copy_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // copy to clipboard
            int retries = 0;
            bool isSuccess = false;
            while (retries++ < 5)
            {
                try
                {
                    Clipboard.SetText(textbox.Text);
                    isSuccess = true;
                    break;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            if (!isSuccess)
            {
                MessageBox.Show("Failed to copy to clipboard. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
