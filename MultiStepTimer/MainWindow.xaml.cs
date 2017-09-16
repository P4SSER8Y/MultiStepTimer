using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiStepTimer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int test;
        public MainWindow()
        {
            Controls.Parent = this;
            InitializeComponent();
            Controls.AfterParentInit();
            ReadConfig();
        }

        private void ReadConfig()
        {
            this.slider.Value = 2;
            Controls.Timeout[0].Value = 1;
            Controls.Timeout[1].Value = 1;
            Controls.RemainTime[1].Value = 0.5;
            Controls.Update(1.5);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Controls.Size = (int) e.NewValue;
        }

        private void slider_Loaded(object sender, RoutedEventArgs e)
        {
            Controls.Size = (int) (sender as Slider).Value;
        }

        private bool _isRunning = false;
        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                Timer.Stop();
                this.ButtonStartStop.Content = "Start";
                Controls.Enable();
            }
            else
            {
                Timer.Start();
                this.ButtonStartStop.Content = "Stop";
                Controls.Disable();
            }
            _isRunning = !_isRunning;
        }
        
    }
}
