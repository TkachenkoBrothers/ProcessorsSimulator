using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProcessorsSimulator
{
    class TimeCalculator
    {
        DateTime firstTime;
        DateTime secondTime;
        public void SetFirstTime()
        {
            firstTime = DateTime.Now;
        }

        public void SetSecondTime()
        {
            secondTime = DateTime.Now;
        }

        public int TimeOfWork()
        {
            int minutes = secondTime.Minute - firstTime.Minute;
            int seconds = secondTime.Second - firstTime.Second;
            int miliseconds = secondTime.Millisecond - firstTime.Millisecond;
            return minutes * 60 * 1000 + seconds * 1000 + miliseconds;
        }
    }
}
