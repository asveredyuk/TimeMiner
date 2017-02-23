using System.Collections.Generic;

namespace TimeMiner.Master
{
    public interface IResourceContainer
    {
        byte[] GetResource(string key);
        string GetString(string key);
    }
}