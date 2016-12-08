using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave
{
    /// <summary>
    /// Class for working with winapi
    /// </summary>
    class WinapiLogger
    {
        private static WinapiLogger self;

        public static WinapiLogger Self
        {
            get
            {
                if (self == null)
                    return self;
                return self;
            }
        }
        private WinapiLogger()
        {

        }

        ~WinapiLogger()
        {
            UnHook();
        }
        /// <summary>
        /// Reset counters in logger
        /// </summary>
        public void Reset()
        {
            
        }
        /// <summary>
        /// Hook to windows
        /// </summary>
        public void Hook()
        {
            
        }
        /// <summary>
        /// Uhook from windows
        /// </summary>
        public void UnHook()
        {
            
        }

       
    }
}
