using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Util
{
    public class MovingAverage
    {
        public static int Calculate(List<int> list, int newListEntry, int interval)
        {
            int mavg = 0;
            list.Add(newListEntry);
            if (list.Count == interval)
            {
                mavg = (int)Math.Ceiling((double)list.Sum() / interval);
                list.RemoveAt(0);
            }
            return mavg;
        }
    }
}
