using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Core
{
    public class WindowDescriptor
    {
        /// <summary>
        /// Title of the window
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Position of the window
        /// </summary>
        public IntPoint Location { get; set; }
        /// <summary>
        /// Size of the window (x = width, y = height)
        /// </summary>
        public IntPoint Size { get; set; }

        public override string ToString()
        {
            return $"Title: {Title}, Location: {Location}, Size: {Size}";
        }
    }
}
