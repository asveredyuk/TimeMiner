using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeMiner.Master.Database;

namespace TimeMiner.Master.Analysis
{
    /// <summary>
    /// Base class for all reports with customizable parameters
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    /// <typeparam name="Q">Param type</typeparam>
    public abstract class ParametrizedBaseReport<T,Q> : BaseReport<T> where T: BaseReportResult
    {
        protected Q parameters;

        public Q Parameters
        {
            get { return parameters; }
        }
        protected ParametrizedBaseReport(ILog log) : base(log)
        {
        }

        protected override string GetParamsHash()
        {
            string json = JsonConvert.SerializeObject(parameters);
            string paramsMd5 = Util.ComputeStringMD5Hash(json);
            return paramsMd5;
        }
    }
}
