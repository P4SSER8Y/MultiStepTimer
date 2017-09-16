using System;
using System.Timers;

namespace MultiStepTimer
{
    static class Timer
    {
        private static readonly System.Timers.Timer _timer;

        private static DateTime t0;

        static Timer()
        {
            _timer = new System.Timers.Timer(40);
            _timer.Stop();
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += Touch;
        }

        private static void Touch(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var tmp = DateTime.Now - t0;
            Controls.Update(tmp.TotalSeconds);
        }

        public static void Stop()
        {
            _timer.Stop();
        }

        public static void Start()
        {
            t0 = DateTime.Now;
            _timer.Start();
        }
    }
}
