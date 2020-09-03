using System;
using System.Timers;

namespace Backend
{
    public class GateWithTimer
    {
        public int WaitTime { get; }
        private Timer timer;
        private string key;
        public bool isOpened;

        public GateWithTimer(int waitTime)
        {
            WaitTime = waitTime;
            this.timer = new Timer(waitTime);
            this.timer.AutoReset = false;
            this.timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            isOpened = true;
        }

        public bool Open(string key)
        {
            if (this.key != key || isOpened)
            {
                Close(key);
            }
            return false;
        }

        private void Close(string key)
        {
            this.key = key;
            this.isOpened = false;
            this.timer.Stop();
            this.timer.Start();
        }
    }
}
