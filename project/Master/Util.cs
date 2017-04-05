using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master
{
    static class Util
    {
        public static bool CheckPeriodsIntercept(DateTime aBegin, DateTime aEnd, DateTime bBegin, DateTime bEnd)
        {
            return aBegin < bEnd && bBegin < aEnd;
        }
        public static bool CheckDateInPeriod(DateTime date, DateTime periodBegin, DateTime periodEnd)
        {
            return periodBegin <= date && date <= periodEnd;
        }
    }
}
