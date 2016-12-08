using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TimeMiner.Core
{
    public class IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntPoint()
        {
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }
    }
}
