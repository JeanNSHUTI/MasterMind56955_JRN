using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISIBToolbox
{
    public enum TimerMode { COUNT_DOWN, COUNT_UP };
    public class TimerLabel:Label
    {
        private int nbSeconds;
        private Timer clockTimer = new Timer();

        public event EventHandler<ThresholdReachedEventArgs> ThresholdReached;
        public event EventHandler TimeOut;

        public override string Text { get { return getTimeHHMMSS(); } }
        public TimerMode Mode { get; set; }
        public int Threshold { get; set; }
        public int Hours { get { return nbSeconds / 3600 % 60; } set { setNbSeconds(value, Minutes, Seconds); } }
        public int Minutes { get { return nbSeconds / 60 % 60; } set { setNbSeconds(Hours, value, Seconds); } }
        public int Seconds { get { return nbSeconds % 60; } set { setNbSeconds(Hours, Minutes, value); } }

        private void setNbSeconds(int h, int m, int s)
        {
            nbSeconds = h * 3600 + m * 60 + s;
        }

        public TimerLabel():this(0)
        {
            
        }
        
        public TimerLabel(int s)
        {
            nbSeconds = s;
            clockTimer.Interval = 1000;
            clockTimer.Tick += ClockTimer_Tick;
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            switch (Mode)
            {
                case TimerMode.COUNT_DOWN: nbSeconds--;
                    break;
                case TimerMode.COUNT_UP:nbSeconds++;
                    break;
                default:
                    break;
            }
            if (nbSeconds < 0)
                nbSeconds = 0;
            
            this.Invalidate();

            if (nbSeconds - Threshold == 0)
                onThresholdReached();
            if (nbSeconds == 0)
                onTimeOut();

        }

        private void onTimeOut()
        {
            clockTimer.Enabled = false;
            if(TimeOut!=null)
            {
                TimeOut(this, EventArgs.Empty);
            }
        }

        private void onThresholdReached()
        {
            if(ThresholdReached != null)
            {
                ThresholdReachedEventArgs e = new ThresholdReachedEventArgs();
                e.Value = nbSeconds;
                ThresholdReached(this, e);
            }
        }

        public void StartTimer()
        {
            clockTimer.Enabled = true;
        }
        public void StopTimer()
        {
            clockTimer.Enabled = false;
        }

        private string getTimeHHMMSS()
        {
            string time = "";

            int s = nbSeconds % 60;
            int m = (nbSeconds / 60) % 60;
            int h = (nbSeconds / 3600) % 100;

            if (h < 10)
                time += "0";
            time += h.ToString()+":";
            if (m < 10)
                time += "0";
            time += m.ToString() + ":";
            if (s < 10)
                time += "0";
            time += s.ToString();
            
            return time;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
