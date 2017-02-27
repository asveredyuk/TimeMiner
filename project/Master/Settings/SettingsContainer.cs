using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using TimeMiner.Master.Database;

namespace TimeMiner.Master.Settings
{
    public class SettingsContainer
    {
        private static SettingsContainer self;

        public static SettingsContainer Self
        {
            get
            {
                if (self == null)
                {
                    self = new SettingsContainer();
                }
                return self;
            }
        }

        public const string APPS_LIST_COL_NAME = "apps";
        public const string PROFILE_COL_PREFIX = "profiledata_";
        public const string PROFILES_COL_NAME = "profiles";
        private LiteDatabase db;
        private LiteCollection<ApplicationDescriptor> appsCollection;
        private LiteCollection<ProfileApplicationRelevance> baseProfileReferences;
        private SettingsContainer()
        {
            
            db = MasterDB.Settings.Database;
            appsCollection = db.GetCollection<ApplicationDescriptor>(APPS_LIST_COL_NAME);
            baseProfileReferences = db.GetCollection<ProfileApplicationRelevance>(PROFILE_COL_PREFIX + "base");
        }
        /// <summary>
        /// Get base profile with its relevances
        /// </summary>
        /// <returns></returns>
        public Profile GetBaseProfile()
        {
            Profile p = new Profile();
            p.Id = Guid.Empty;
            p.Name = "Base profile";
            p.Relevances = new List<ProfileApplicationRelevance>(baseProfileReferences.Include(t=>t.App).FindAll());
            return p;
        }
        /// <summary>
        /// Update relevance in base profile
        /// </summary>
        /// <param name="baseRelevance"></param>
        public void UpdateRelevance(ProfileApplicationRelevance baseRelevance)
        {
            if (!baseProfileReferences.Update(baseRelevance))
            {
                throw new ArgumentException("No relevance with such id found");
            }
        }
        /// <summary>
        /// Add new application and its relevance
        /// </summary>
        /// <param name="baseRelevance"></param>
        public void PutNewApp(ProfileApplicationRelevance baseRelevance)
        {
            appsCollection.Insert(baseRelevance.App);
            baseProfileReferences.Insert(baseRelevance);
        }
        /// <summary>
        /// Update application descriptor
        /// </summary>
        /// <param name="desc"></param>
        public void UpdateApp(ApplicationDescriptor desc)
        {
            if (!appsCollection.Update(desc))
            {
                //this description was not found
                throw new ArgumentException("Descriptor was not found");
            }
        }

        /// <summary>
        /// Remove aplication and all references to it
        /// </summary>
        /// <param name="id"></param>
        public void RemoveAppAnRelevances(Guid id)
        {
            //remove all references
            baseProfileReferences.Delete(t => t.App.Id == id);
            //remove app
            if (!appsCollection.Delete(id))
            {
                throw new ArgumentException("App not found");
            }

        }

        /*public List<Profile> GetProfileSkeletons()
        {
            List<Profile> p = new List<Profile>(db.GetCollection<Profile>(PROFILES_COL_NAME).FindAll());
            return p;
        }

        public void FulfillProfile(Profile skeleton)
        {
            if (skeleton.Relevances.Count != 0)
            {
                throw new ArgumentException("Given profile is not skeleton");
            }
            if (!db.CollectionExists(PROFILE_COL_PREFIX + skeleton.Id))
            {
                throw new Exception("This skeleton is invalid");
            }
            List<ProfileApplicationRelevance> relevances = new List<ProfileApplicationRelevance>();
            //temp solution - only base profile works
        }*/

        

    }
}
