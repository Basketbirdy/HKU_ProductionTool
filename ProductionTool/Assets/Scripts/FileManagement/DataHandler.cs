using JetBrains.Annotations;
using UnityEngine;

namespace FileManagement
{
    public class DataHandler
    {
        public DataHandler() 
        {
            
        }

        /// <summary>
        /// method that creates a fresh new instance of data with default values
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public DataHolder CreateFreshProject(string url)
        {
            // TODO - create instance of SavaData class filled with default values and provided image
            Debug.LogWarning($"TODO - Implement creating fresh project data");
            return null;
        }
    }
}
