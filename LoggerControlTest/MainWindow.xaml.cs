using LoggerControlTest.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LoggerControlTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScrollViewer _scrollViewer;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Add_Log(object sender, EventArgs e)
        {
            vm.LogItemList.Add(new LogItemModel()
            {
                Message = "HHH",
                Time = DateTime.Now.ToString(),
                Category = "Debug"
            });

            if (isAutoScroll.IsChecked.HasValue && isAutoScroll.IsChecked.Value)
            {
                _scrollViewer?.ScrollToEnd();
            }
        }
        private void Add_Log_2000(object sender, EventArgs e)
        {
            int i = 0;
            while (i < 200000)
            {
                i++;
                vm.LogItemList.Add(new LogItemModel()
                {
                    Message = "HHHfagagadrthathhttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt" +
                    "ttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt",
                    Time = DateTime.Now.ToString(),
                    Category = "Debug"
                });
                if (isAutoScroll.IsChecked.HasValue && isAutoScroll.IsChecked.Value)
                {
                    _scrollViewer?.ScrollToEnd();
                }
            }
        }
        private void Minus_100(object sender, EventArgs e)
        {
            int i = 0;
            while (i < 100)
            {
                i++;
                vm.LogItemList.RemoveAt(0);
            }
        }

        private void ScrollViewer_Loaded(object sender, EventArgs e)
        {
            if (sender is ScrollViewer scroll)
            {
                _scrollViewer = scroll;
            }
        }

    }
}
