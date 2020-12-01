using Go2Share.General.Resources.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Go2Share.General.Resources.Services
{
    public class ResourceService : IResourceService
    {
        //private IEnumerable<ApplicationResource> ApplicationResourceCollectionList;
        public List<ApplicationResource> ApplicationResourceCollectionList;
        private IConfiguration _configuration { get; set; }
        public ResourceService(IConfiguration configuration)
        {
            _configuration = configuration;
            this.LoadCollection();
        }
        public string GetString(string key, Parameters messageParameters)
        {
            var result = "";
            var stringCollection = ApplicationResourceCollectionList;
            if (ApplicationResourceCollectionList.Any())
            {

                var appString = stringCollection.FirstOrDefault(p => p.Key == key);
                result = appString.Value.ToString().Replace(messageParameters.Name, messageParameters.Value);

            }
            return result;
        }
        public string GetString(string key)
        {
            var results = "";
            if (ApplicationResourceCollectionList.Any())
            {
                results = ApplicationResourceCollectionList.FirstOrDefault(o => o.Key == key).Value;
            }
            return results;
        }
        public string GetString(string key, List<Parameters> messageParameters)
        {
            var result = "";
            if (messageParameters != null)
            {
                var appString = ApplicationResourceCollectionList.FirstOrDefault(p => p.Key == key).Value;
                foreach (var replacement in messageParameters)
                {
                    result = appString.ToString().Replace(replacement.Name, replacement.Value);
                    appString = result;
                }

            }
            return result;
        }
        /// <summary>
        /// Collection Added to List for all keys
        /// </summary>
        /// <param name="IsJsonFile"></param>
        public void LoadCollection(bool IsJsonFile = true)
        {
            var jsonFilePathSetting = _configuration["CommonMessagesJsonFile:FilePath"];
            string solutiondir = Directory.GetParent(
                        Directory.GetCurrentDirectory()).Parent.FullName;

            var currDir = Directory.GetCurrentDirectory();

            currDir = currDir + _configuration["resourceFilePath"];
            string[] filePaths = null;
            if (!Directory.Exists(currDir))
            {
                currDir = solutiondir + "\\Juhi_Go2share\\Go2Share.General" + _configuration["resourceFilePath"];// \\Resources\\Files";
            }
            filePaths = Directory.GetFiles(currDir, "*",
                                          SearchOption.AllDirectories);

            ApplicationResourceCollectionList = new List<ApplicationResource>();
            for (int i = 0; i < filePaths.Length; i++)
            {
                using (StreamReader r = new StreamReader(filePaths[i]))
                {
                    string json = r.ReadToEnd();

                    var AppData = (IList<ApplicationResource>)JsonConvert.DeserializeObject(json, typeof(IList<ApplicationResource>));

                    if (AppData != null)
                        ApplicationResourceCollectionList.AddRange(AppData);
                }
            }         

        }
        public ApplicationResource GetApplicationResource(string key)
        {
            ApplicationResource ApplicationResource = new ApplicationResource();
            if (ApplicationResourceCollectionList.Any())
            {
                ApplicationResource = ApplicationResourceCollectionList.FirstOrDefault(o => o.Key == key);
            }
            return ApplicationResource;
        }


    }
}
