using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeMiner.Core;

namespace TimeMiner.Slave
{
    class MasterBoundary
    {
        private static MasterBoundary self;

        public static MasterBoundary Self
        {
            get
            {
                if (self == null)
                {
                    self = new MasterBoundary();
                }
                return self;
            }
        }

        public delegate void OnRecordSentHandler(LogRecord record);

        public event OnRecordSentHandler onRecordSent;
        const string URL = "http://localhost:13000";
        private MasterBoundary()
        {
            
        }

        private void RaiseOnRecordSent(LogRecord recrd)
        {
            if (onRecordSent != null)
            {
                onRecordSent(recrd);
            }
        }
        public async void SendOne(LogRecord record)
        {
            try
            {
                string data = JsonConvert.SerializeObject(record);
                HttpWebRequest req = HttpWebRequest.CreateHttp(URL);
                req.Method = "POST";
                using (StreamWriter sw = new StreamWriter(await req.GetRequestStreamAsync()))
                {
                    await sw.WriteLineAsync(data);
                    sw.Close();
                    HttpWebResponse resp = (HttpWebResponse)(await req.GetResponseAsync());
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        //successfully sent
                        RaiseOnRecordSent(record);
                    }
                    //log was not successfully sent
                }
            }
            catch (Exception e)
            {
                //log was not successfully sent
            //    throw;
            }
        }

    }
}
