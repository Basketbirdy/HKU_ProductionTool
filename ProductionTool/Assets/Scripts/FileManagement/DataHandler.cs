using JetBrains.Annotations;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace FileManagement
{
    public class DataHandler
    {
        private DataHolder defaultData;

        public DataHandler(DataHolder defaultData) 
        {
            this.defaultData = defaultData;
        }

        /// <summary>
        /// method that creates a fresh new instance of data with default values
        /// </summary>
        /// <param name="url"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public DataHolder CreateFreshProject(string url, string version)
        {
            // TODO - create instance of SavaData class filled with default values and provided image
            Debug.LogWarning($"TODO - Implement creating fresh project data");

            // create metadata
            DataHeader metaData = new DataHeader(defaultData.metaData);
            metaData.version = version;
            metaData.date = DateTime.Now.ToString();

            // create data instance
            DataHolder newProjectData = new DataHolder(defaultData);
            newProjectData.metaData = metaData;

            newProjectData.fileName = Path.GetFileName(url);
            newProjectData.originalTexture = TextureUtils.LoadImage(url);

            return newProjectData;
        }
    }
}
