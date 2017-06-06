using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeMiner.Slave.BuiltinExtensions
{
    public class PersonalStatisticsExtension: ClientInterfaceExtension
    {
        [MenuItem("Personal stats",9)]
        public async void GetPersonalStats()
        {
            var stats = await MasterBoundary.Self.GetPersonalStatistics();
            string res = $"Productive time : {TimeSpan.FromSeconds(stats.ProductiveTime)}\n" +
                         $"Distractions time : {TimeSpan.FromSeconds(stats.DistractionsTime)}\n" +
                         $"Total time : {TimeSpan.FromSeconds(stats.TotalTime)}";
            MessageBox.Show(res,"Statistics for today");
        }
    }
}
