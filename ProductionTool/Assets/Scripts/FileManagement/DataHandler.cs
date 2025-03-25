using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FileManagement
{
    public class DataHandler
    {
        private DataHolder defaultData;

        private DataHolder activeData;

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
        public DataHolder CreateFreshProject(string url)
        {
            // TODO - create instance of SavaData class filled with default values and provided image
            //Debug.LogWarning($"TODO - Implement creating fresh project data");

            // create data instance
            DataHolder newProjectData = (DataHolder)ScriptableObject.CreateInstance("DataHolder");
            newProjectData.OnCreateDataHolder(defaultData);

            newProjectData.fileName = Path.GetFileName(url);
            newProjectData.originalTexture = TextureUtils.LoadImage(url);

            // identify and store all unique colors
            newProjectData.originalColors = TextureUtils.GetUniqueColors(newProjectData.originalTexture);
            newProjectData.originalColors = SortColors(newProjectData.originalColors);

            newProjectData.colorVariants = new List<ColorVariant>();
            newProjectData.colorVariants.Add(new ColorVariant("DefaultVariant", newProjectData.originalColors));

            return newProjectData;
        }

        public DataHeader CreateMetadata(string version)
        {
            // create metadata
            string date = System.DateTime.Now.ToString("yyyyMMddHHmmss"); ;
            DataHeader metadata = new DataHeader(version, date);
            //DataHeader metaData = (DataHeader)ScriptableObject.CreateInstance("DataHeader");
            //metaData.version = version;
            //metaData.date = System.DateTime.Now.ToString();
            return metadata;
        }

        private Color[] SortColors(Color[] colors)
        {
            float H;
            float h;
            Color temp;

            // bubble sort - Array is always < 256 so bubblesort performance impact is minimal.
            for(int i = colors.Length - 1; i > 0; i--)
            {
                for(int j = 0; j < colors.Length - 1; j++) 
                {
                    Color.RGBToHSV(colors[j], out H, out float S, out float V);
                    Color.RGBToHSV(colors[j + 1], out h, out float s, out float v);
                    if(H > h)
                    {
                        // swap
                        temp = colors[j + 1];
                        colors[j + 1] = colors[j];
                        colors[j] = temp;
                    }
                }
            }

            return colors;
        }
    }
}
