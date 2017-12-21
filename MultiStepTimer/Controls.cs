using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;

namespace MultiStepTimer
{
    public class BindingValue<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    static class Controls
    {
        public static MainWindow Parent = null;
        private static readonly TextBox[] _titles = new TextBox[App.MaxNumOfSteps];
        private static readonly ProgressBar[] _progressBars = new ProgressBar[App.MaxNumOfSteps];
        private static readonly Label[] _remainTime = new Label[App.MaxNumOfSteps];
        public static readonly TextBox[] _timeoutConfig = new TextBox[App.MaxNumOfSteps];

        public static BindingValue<string>[] Title = new BindingValue<string>[App.MaxNumOfSteps];
        public static BindingValue<double>[] RemainTime = new BindingValue<double>[App.MaxNumOfSteps];
        public static BindingValue<double>[] Timeout = new BindingValue<double>[App.MaxNumOfSteps];

        public static BindingValue<string> Count = new BindingValue<string>();
        public static BindingValue<string> Status = new BindingValue<string>();
        public static BindingValue<double> TotalRemaining = new BindingValue<double>();
        public static BindingValue<double> TaskbarProgress = new BindingValue<double>();

        private static readonly SolidColorBrush _progressBarGreen = new SolidColorBrush(Color.FromRgb(6, 176, 37));
        private static readonly SolidColorBrush _progressBarRed = new SolidColorBrush(Color.FromRgb(232, 0, 0));

        class TimeoutValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                double temp = 0.0;
                if (double.TryParse(value as string, out temp))
                {
                    if (temp > 0)
                    {
                        return new ValidationResult(true, null);
                    }
                }
                return new ValidationResult(false, "Time should be positive");
            }
        }

        class RemainingTimeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return $"{(double)value:F1}";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        class TimeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return $"{(double)value:F1}";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        static Controls()
        {
            for (var i = 0; i < App.MaxNumOfSteps; i++)
            {
                Title[i] = new BindingValue<string>() { Value = $"Step #{i+1:00}" };
                _titles[i] = new TextBox
                {
                    Text = $"第{i+1}步",
                    TextAlignment = TextAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 14,
                    MinWidth = 80,
                    Margin = new Thickness(1),
                };
                var bind = new Binding("Value")
                {
                    Source = Title[i],
                    Mode = BindingMode.TwoWay,
                };
                _titles[i].SetBinding(TextBox.TextProperty, bind);
                Grid.SetColumn(_titles[i], 0);
                Grid.SetRow(_titles[i], i);

                RemainTime[i] = new BindingValue<double>() {Value = 0.1};
                _remainTime[i] = new Label
                {
                    Content = "1.0",
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 14,
                    Width = 50,
                    Visibility = Visibility.Collapsed,
                    Margin = new Thickness(1),
                    Padding = new Thickness(0),
                    BorderThickness = new Thickness(1),
                };
                var bindTitle = new Binding("Value")
                {
                    Source = RemainTime[i],
                    Mode = BindingMode.TwoWay,
                    Converter = new RemainingTimeConverter(),
                };
                _remainTime[i].SetBinding(Label.ContentProperty, bindTitle);
                Grid.SetColumn(_remainTime[i], 2);
                Grid.SetRow(_remainTime[i], i);

                Timeout[i] = new BindingValue<double>() {Value = 1};
                _timeoutConfig[i] = new TextBox
                {
                    Text = Timeout[i].Value.ToString(),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 50,
                    FontSize = 14,
                    Margin = new Thickness(1),
                };
                var i1 = i;
                _timeoutConfig[i].TextChanged += delegate(object sender, TextChangedEventArgs args)
                {
                    var temp = 0.0;
                    if (double.TryParse((sender as TextBox).Text, out temp))
                    {
                        if (temp > 0)
                        {
                            Parent.ButtonStartStop.IsEnabled = true;
                            Parent.ButtonStartStop.Content = "Start";
                            _progressBars[i1].Foreground = _progressBarGreen;
                            Timeout[i1].Value = temp;
                            RemainTime[i1].Value = temp;
                            return;
                        }
                    }
                    Parent.ButtonStartStop.IsEnabled = false;
                    Parent.ButtonStartStop.Content = "ERROR";
                    _progressBars[i1].Foreground = _progressBarRed;
                    Timeout[i1].Value = 1.0;
                    RemainTime[i1].Value = 1.0;
                };
                Grid.SetColumn(_timeoutConfig[i], 2);
                Grid.SetRow(_timeoutConfig[i], i);


                _progressBars[i] = new ProgressBar()
                {
                    Minimum = 0,
                    Value = 0,
                    MinWidth = 150,
                    Margin = new Thickness(5, 1, 5, 1),
                    FlowDirection = FlowDirection.RightToLeft,
                };
                var bindPbarMax = new Binding("Value")
                {
                    Source = Timeout[i],
                    Mode = BindingMode.OneWay
                };
                _progressBars[i].SetBinding(ProgressBar.MaximumProperty, bindPbarMax);
                var bindPbarValue = new Binding("Value")
                {
                    Source = RemainTime[i],
                    Mode = BindingMode.OneWay,
                };
                _progressBars[i].SetBinding(ProgressBar.ValueProperty, bindPbarValue);
                Grid.SetColumn(_progressBars[i], 1);
                Grid.SetRow(_progressBars[i], i);
            }
        }

        public static void AfterParentInit()
        {
            if (Parent == null)
                throw new NullReferenceException();
            var bindMainPbar = new Binding("Value")
            {
                Source = TotalRemaining,
                Mode = BindingMode.OneWay,
            };
            Parent.MainProgressBar.SetBinding(ProgressBar.ValueProperty, bindMainPbar);

            var bindLabelStatus = new Binding("Value")
            {
                Source = Status,
                Mode = BindingMode.TwoWay,
            };
            Parent.LabelStatus.SetBinding(Label.ContentProperty, bindLabelStatus);

            var bindLabelCount = new Binding("Value")
            {
                Source = Count,
                Mode = BindingMode.TwoWay,
            };
            Parent.LabelCount.SetBinding(Label.ContentProperty, bindLabelCount);
        }

        private static double _totalTimeout = 1.0;
        private static void Sum()
        {
            _totalTimeout = 0.0;
            for (var i = 0; i < Size; i++)
                _totalTimeout += Timeout[i].Value;
            Parent.MainProgressBar.Maximum = _totalTimeout;
        }

        private static int _size;
        public static int Size
        {
            get {return _size;}
            set
            {
                _size = value;

                if (Parent.MainGrid == null) return;
                Parent.MainGrid.Children.Clear();
                for (var i = 0; i < _size; i++)
                {
                    Parent.MainGrid.Children.Add(_titles[i]);
                    Parent.MainGrid.Children.Add(_progressBars[i]);
                    Parent.MainGrid.Children.Add(_remainTime[i]);
                    Parent.MainGrid.Children.Add(_timeoutConfig[i]);
                }
            }
        }
        
        public static void Enable()
        {
            for (var i = 0; i < App.MaxNumOfSteps; i++)
            {
                _titles[i].IsEnabled = true;
                _remainTime[i].Visibility = Visibility.Collapsed;
                _timeoutConfig[i].Visibility = Visibility.Visible;
            }
            Parent.slider.IsEnabled = true;
        }

        public static void Disable()
        {
            Sum();
            for (var i = 0; i < App.MaxNumOfSteps; i++)
            {
                _titles[i].IsEnabled = false;
                _remainTime[i].Visibility = Visibility.Visible;
                _timeoutConfig[i].Visibility = Visibility.Collapsed;
            }
            Parent.slider.IsEnabled = false;
        }

        
        public static void Update(double totalSeconds)
        {
            var count = (int)Math.Floor(totalSeconds / _totalTimeout);
            var remaining = totalSeconds % _totalTimeout;
            TotalRemaining.Value = (count % 2 == 0) ? remaining : _totalTimeout - remaining;
            Status.Value = $"{(int)Math.Floor(totalSeconds/60):d2}:{totalSeconds%60:00.0}";
            Count.Value = $"{count:00}";
            
            for (var i = 0; i < Size; i++)
            {
                if (remaining > Timeout[i].Value)
                {
                    RemainTime[i].Value = 0;
                    remaining -= Timeout[i].Value;
                }
                else
                {
                    RemainTime[i].Value = Timeout[i].Value - remaining;
                    remaining = 0;
                }
            }
        }
    }
}
