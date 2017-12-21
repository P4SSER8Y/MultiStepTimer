using Microsoft.Win32;
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
        public MainWindow()
        {
            Controls.Parent = this;
            InitializeComponent();
            Controls.AfterParentInit();
            ReadConfig();

            var args = Environment.GetCommandLineArgs();
            if (args.Count() > 1)
                LoadConfig(args[1]);
        }

        private void ReadConfig()
        {
            this.slider.Value = 2;
            Controls.Title[0].Value = "Hello";
            Controls.Title[1].Value = "World";
            Controls.Timeout[0].Value = 2;
            Controls.Timeout[1].Value = 1;
            Controls.Update(0);
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
                this.Topmost = false;
                this.AllowDrop = true;
                this.ButtonOpenConfig.IsEnabled = true;
                Controls.Enable();
            }
            else
            {
                Timer.Start();
                this.ButtonStartStop.Content = "Stop";
                this.ButtonOpenConfig.IsEnabled = false;
                this.AllowDrop = false;
                this.Topmost = true;
                Controls.Disable();
            }
            _isRunning = !_isRunning;
        }

        private void ButtonOpenConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Yaml配置文件(*.yaml)|*.yaml";
            dialog.ValidateNames = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            var flag = dialog.ShowDialog();
            if (flag == true)
                LoadConfig(dialog.FileName);
        }

        public void LoadConfig(string filename)
        {
            Console.WriteLine(filename);
            Config cfg;
            try
            {
                cfg = ConfigReader.Read(filename);
            }
            catch
            {
                return;
            }
            
            var n = cfg.items.Count();
            if (n < 1)
                return;
            this.slider.Value = n;
            for (var i = 0; i < n; i++)
            {
                Controls.Title[i].Value = cfg.items[i].title;
                Controls._timeoutConfig[i].Text = $"{cfg.items[i].timeout}";
            }
            Controls.Update(0);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadConfig(files[0]);
        }
    }
}
