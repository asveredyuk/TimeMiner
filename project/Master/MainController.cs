using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master
{
    /// <summary>
    /// Main controller class
    /// </summary>
    class MainController
    {
        private static MainController self;
        public static MainController Self
        {
            get
            {
                if (self == null)
                    self = new MainController();
                return self;
            }
        }
    }
}
