using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Core.MetaInfoClasses
{
    /// <summary>
    /// Describes offline-activity, added with "offline_activity" keyword
    /// </summary>
    public class OfflineActivity
    {
        public const string TAG = "offline_activity";
        public enum ActivityType
        {
            work,
            nonWork
        }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public ActivityType Relevance { get; set; }

        public OfflineActivity()
        {
        }

        public OfflineActivity(DateTime begin, DateTime end, string shortName, string description, ActivityType relevance)
        {
            Begin = begin;
            End = end;
            ShortName = shortName;
            Description = description;
            Relevance = relevance;
        }
    }
}
