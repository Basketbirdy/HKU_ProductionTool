using UnityEngine;
using System.IO;
using System;

namespace FileManagement
{
    public class SaveHandler
    {

        public void Save(string url, DataHolder data, string version)
        {
            if(url == "" || url == null) { Debug.Log("No path provided! returning"); return; }
            Debug.Log($"Saving to path: {url}");

            // create and fill out save data class
            SaveData savedata = new SaveData();

            string texture = TextureUtils.TextureToString(data.originalTexture);

            savedata.Initialize(
                new DataHeader(version, System.DateTime.Now.ToString("yyyyMMddHHmmss")),
                data.fileName,
                texture,
                data.originalColors,
                data.colorVariants.ToArray()
                );

            string stringifiedData = JsonUtility.ToJson(savedata, true);
            Debug.Log(stringifiedData.ToString());

            WriteToFile(url, stringifiedData);
        }

        private void WriteToFile(string url, string data)
        {
            StreamWriter streamWrites = new StreamWriter(url, false);
            streamWrites.WriteLine(data);
            streamWrites.Close();
            streamWrites.Dispose();
        }
    }
}
