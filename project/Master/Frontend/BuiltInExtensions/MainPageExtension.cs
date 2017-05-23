using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    
    class MainPageExtension:FrontendServerExtensionBase
    {
        private FrontendPageMenuItem ite;
        public MainPageExtension()
        {
            /*ite = new FrontendPageMenuItem("Main");
            ite.Path = "/";
            ite.InnerItems.Add(new FrontendPageMenuItem("Google","http://google.ru"));
            ite.InnerItems.Add(new FrontendPageMenuItem("Yandex", "http://yandex.ru"));
            MenuItems.Add(ite);*/
        }
        [PublicHandler]
        [HandlerPath("")]
        public HandlerPageDescriptor Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string token = GetTokenFromRequest(req);
            bool authorized = Authorization.Self.ValidateToken(token);
            if (!authorized)
            {
                resp.Redirect("/login");
                resp.Close();
                return null;
            }
            return new HandlerPageDescriptor("<h1>Welcome to TimeMiner</h1>");
        }
    }
}
