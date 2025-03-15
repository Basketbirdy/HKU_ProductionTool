using UnityEngine;
using System.IO;

namespace FileManagement
{
    public class SaveHandler
    {
        private string extension;

        public SaveHandler(string extension) { this.extension = extension; }

        public void Save(string url, DataHolder data, DataHeader metadata)
        {
            Debug.LogWarning("DOING - Converting data to string and saving it at specified url");

            // create and fill out save data class
            SaveData savedata = new SaveData();

            string texture = TextureToString(data.originalTexture);

            savedata.Initialize(
                metadata,
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
            if(File.Exists(url))
            {
                url = Path.GetFileNameWithoutExtension(url);
            }
            StreamWriter streamWrites = new StreamWriter(url + extension, false);
            streamWrites.WriteLine(data);
            streamWrites.Close();
            streamWrites.Dispose();
        }

        private string TextureToString(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            string byteString = System.Convert.ToBase64String(bytes);
            Debug.Log($"Bytestring: {byteString}");
            return byteString;
        }
    }
}
