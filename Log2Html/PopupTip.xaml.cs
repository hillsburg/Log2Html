using System;
using System.Windows;
using System.Windows.Threading;

namespace Log2Html
{
    /// <summary>
    /// Interaction logic for PopupTip.xaml
    /// </summary>
    public partial class PopupTip : Window
    {
        public PopupTip(string tip)
        {
            InitializeComponent();
            PopupTipTextBlock.Text = tip;

            // Start a timer to close the window after 2 seconds (2000 ms)
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
        }
    }
}
