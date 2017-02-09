﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend
{
    
    class LoginExtension : FrontendServerExtensionBase
    {
        public LoginExtension()
        {
            MenuItems.Add(new TemplatePageMenuItem("Login") {Path = "/login"});
        }
        [HandlerPath("login")]
        public HandlerPageDescriptor Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new HandlerPageDescriptor("login!");
        }

        
    }
}