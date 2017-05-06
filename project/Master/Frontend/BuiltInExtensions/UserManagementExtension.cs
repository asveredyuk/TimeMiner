using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TimeMiner.Master.Database;
using TimeMiner.Master.Frontend.Plugins;
using TimeMiner.Master.Settings;

namespace TimeMiner.Master.Frontend.BuiltInExtensions
{
    public class UserManagementExtension : FrontendServerExtensionBase
    {
        [MenuItem("Users","config/users",12)]
        [HandlerPath("config/users")]
        public HandlerPageDescriptor Handler(HttpListenerRequest req, HttpListenerResponse resp)
        {
            string head = WWWRes.GetString("users/head.html");
            string page = WWWRes.GetString("users/page.html");
            //SettingsDB.Self.UpaserUser(new UserInfo(Guid.Empty, "Alex","Sveredyuk"));
            return new HandlerPageDescriptor(page,head);
        }


        [ApiPath("config/users/gettable")]
        public void GetTable(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var allUsers = SettingsDB.Self.GetAllUsers();
            var json = JsonConvert.SerializeObject(allUsers);
            WriteStringAndClose(resp, json);
        }


        
        [ApiPath("config/users/add")]
        public void AddUser(HttpListenerRequest req, HttpListenerResponse resp)
        {
            //wrong data - 400
            //user already exists - 412
            var json = ReadPostString(req);
            UserInfo info = TryParseUserInfo(json);
            if (info == null)
            {
                WriteStringAndClose(resp,"Wrong data", 400);
                return;
            }
            if (info.Id != Guid.Empty)
            {
                if (SettingsDB.Self.GetUserById(info.Id) != null)
                {
                    WriteStringAndClose(resp,"User with id already exists",412);
                    return;
                }
            }
            //create new id if it is empty
            if(info.Id == Guid.Empty)
                info.Id = Guid.NewGuid();

            SettingsDB.Self.UpaserUser(info);
        }

        private UserInfo TryParseUserInfo(string json)
        {
            JObject obj = JObject.Parse(json);
            if (obj["Name"] == null || obj["Surname"] == null)
            {
                return null;
            }
            string name = obj["Name"].Value<string>();
            string surname = obj["Surname"].Value<string>();
            Guid guid = Guid.Empty;
            if (obj["Id"] != null)
            {
                //guid field exists
                string id = obj["Id"].Value<string>();
                if (id != "" && !Guid.TryParse(id, out guid))
                {
                    //wrong guid
                    return null;
                }
            }
            return new UserInfo(guid, name, surname);
        }
        [ApiPath("config/users/delete")]
        public void DeleteUser(HttpListenerRequest req, HttpListenerResponse resp)
        {
            //400 - no id or it is wrong
            var json = ReadPostString(req);
            JObject obj = JObject.Parse(json);
            Guid guid;
            if (obj["Id"] == null || !Guid.TryParse(obj["Id"].Value<string>(), out guid))
            {
                WriteStringAndClose(resp,"No id specified or it is wrong", 400);
                return;
            }
            SettingsDB.Self.RemoveUser(guid);
        }
    }
}
