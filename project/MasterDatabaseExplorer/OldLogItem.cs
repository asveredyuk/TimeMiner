using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDatabaseExplorer
{
    class OldLogItem
    {
        /// <summary>
        /// Unix timestamp of item captured
        /// </summary>
        public int time;

        /// <summary>
        /// Title of active window
        /// </summary>
        public string activeWindowTitle;

        /// <summary>
        /// Name of process-owner of active window
        /// </summary>
        public string activeWindowProcessName;

        /// <summary>
        /// Mouse position
        /// </summary>
        public Point cursorPos;

        /// <summary>
        /// Count of pressed keys on keyboard
        /// </summary>
        public int keypressCount;

        /// <summary>
        /// Count of actions by mouse (clicks, wheel moves)
        /// </summary>
        public int mouseActionsCount;

        /// <summary>
        /// Extra info (depends of process, e.g. for chrome this is current URL)
        /// </summary>
        public string extraInfo;

        /// <summary>
        /// Link to the previous element, if exitst
        /// </summary>
        public OldLogItem previus;

        public OldLogItem(int time, string activeWindowTitle, string activeWindowProcessName, Point cursorPos,
            int keypressCount, int mouseActionsCount)
        {
            this.time = time;
            if (activeWindowTitle == null)
                activeWindowTitle = "null";
            this.activeWindowTitle = activeWindowTitle.Replace(";", "").Replace("\r", "").Replace("\n", "");
            this.activeWindowProcessName = activeWindowProcessName;
            this.cursorPos = cursorPos;
            this.keypressCount = keypressCount;
            this.extraInfo = "";
            this.mouseActionsCount = mouseActionsCount;
        }

        /// <summary>
        /// Add extra info
        /// </summary>
        /// <param name="info"></param>
        public void PutExtraInfo(string info)
        {
            if (info == null)
            {
                info = "null";
            }
            this.extraInfo = info.Replace(";", "").Replace("\r", "").Replace("\n", "");
        }

        public override string ToString()
        {
            return extraInfo; //keypressCount.ToString(); //cursorPos.ToString();
            //return $"Time: {UnixTimestamp.ConvertToDatetime(time).ToString("T")}, activeWindowTitle: {activeWindowTitle}, activeWindowProcessName: {activeWindowProcessName}";
        }

        /// <summary>
        /// Convert item to csv row
        /// </summary>
        /// <returns></returns>
        public string ToCSVRow()
        {
            return
                $"{time};{activeWindowTitle};{activeWindowProcessName};{cursorPos.X};{cursorPos.Y};{keypressCount};{mouseActionsCount};{extraInfo}";
        }

        /// <summary>
        /// Create new item from csv row (factory)
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static OldLogItem FromCSVRow(string row)
        {
            string[] arr = row.Split(';');
            var item = new OldLogItem(int.Parse(arr[0]), arr[1], arr[2], new Point(int.Parse(arr[3]), int.Parse(arr[4])),
                int.Parse(arr[5]), int.Parse(arr[6]));
            if (arr.Length > 7)
            {
                item.PutExtraInfo(arr[7]);
            }
            return item;
        }
    }
}
