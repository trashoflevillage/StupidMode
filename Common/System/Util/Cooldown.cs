using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidMode.Common.System.Util
{
    internal class Cooldown
    {
        public int maxVal;
        public int val = 0;

        private int defaultVal;

        public Cooldown(int maxVal, int? dv = null)
        {
            if (dv != null)
            {
                val = dv.Value;
            }

            defaultVal = val;
            this.maxVal = maxVal;
        }
        
        /// <summary>
        /// Progresses the cooldown's countdown, then returns true if it reached the maximum value.
        /// If true, the countdown is reset.
        /// </summary>
        /// <returns></returns>
        public bool TickCooldown()
        {
            val++;
            if (val >= maxVal)
            {
                val = 0;
                return true;
            }
            return false;
        }

        public void Reset() {
            val = defaultVal;
        }
    }
}
