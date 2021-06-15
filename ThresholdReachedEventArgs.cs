using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISIBToolbox
{
    public class ThresholdReachedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int Value { get; set; }
    }
}
