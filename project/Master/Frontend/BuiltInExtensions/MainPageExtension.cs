﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimeMiner.Master.Frontend.Plugins;

namespace TimeMiner.Master.Frontend
{
    
    class MainPageExtension:FrontendServerExtensionBase
    {
        private TemplatePageMenuItem ite;
        public MainPageExtension()
        {
            ite = new TemplatePageMenuItem("Main");
            ite.Path = "/";
            ite.InnerItems.Add(new TemplatePageMenuItem("Google","http://google.ru"));
            ite.InnerItems.Add(new TemplatePageMenuItem("Yandex", "http://yandex.ru"));
            MenuItems.Add(ite);
        }
        [HandlerPath("")]
        public HandlerPageDescriptor Handle(HttpListenerRequest req, HttpListenerResponse resp)
        {
            return new HandlerPageDescriptor("ThisIsMainPage");
        }

        
    }
}