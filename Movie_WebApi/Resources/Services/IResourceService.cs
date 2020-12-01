using Movie_WebApi.Resources.Common;
using System;
using System.Collections.Generic;
using System.Text;


namespace Movie_WebApi.Resources.Services
{
    public interface IResourceService
    {
        string GetString(string key, Parameters messageParameters);
        string GetString(string key, List<Parameters> messageParameters);
        string GetString(string key);
        void LoadCollection(bool IsJsonFile);
        ApplicationResource GetApplicationResource(string key);
    }
}
