using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    
    class LoginExtension : FrontendServerExtensionBase
    {
        public LoginExtension()
        {
        }
        [PublicHandler]
        [HandlerPath("login")]
        public HandlerPageDescriptor Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var page = WWWRes.GetString("login/page.html");
            WriteStringAndClose(resp, page);
            //page is written by handler, no descriptor
            return null;
        }
        [PublicHandler]
        [ApiPath("login/gettoken")]
        public void Validate(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var post = ReadPostString(req);
            var jobj = JObject.Parse(post);
            if (jobj["Login"] == null || jobj["Password"] == null)
            {
                WriteStringAndClose(resp, "no login or password",400);
                return;
            }
            string login = jobj["Login"].Value<string>();
            string password = jobj["Password"].Value<string>();
            string token = Authorization.Self.Authorize(login, password);
            if (token!=null)
            {
                
                var res = new
                {
                    Token = token
                };

                var json = JsonConvert.SerializeObject(res);
                WriteStringAndClose(resp, json);
            }
            else
            {
                var res = new
                {
                    Error = "Wrong login or password"
                };
                var json = JsonConvert.SerializeObject(res);
                WriteStringAndClose(resp, json);
            }
        }

        
    }
}
