using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Slave.Hooks
{
    public interface IWinHook
    {
        /// <summary>
        /// Number of actions performed
        /// </summary>
        int ActionsCount { get; }
        /// <summary>
        /// Reset counter
        /// </summary>
        void Reset();
        /// <summary>
        /// Hook to windows
        /// </summary>
        void Hook();
        /// <summary>
        /// Unhook from windows
        /// </summary>
        void UnHook();
    }
}
