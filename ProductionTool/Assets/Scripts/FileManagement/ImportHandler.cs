using UnityEngine;
using System.Collections.Generic;
using SFB;
using System.Windows.Forms;
using Unity.VisualScripting;
using System.IO;

namespace FileManagement
{
    public class ImportHandler
    {
        public ImportHandler() { }

        /// <summary>
        /// Method that interprets data at the provided path and returns it as a useable Data class
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ImportData ImportExistingProject(string path, DataHolder defaultData)
        {
            Debug.LogWarning($"DOING - Implement importing existing project");

            if (!File.Exists(path)) { return new ImportData(); }
            StreamReader streamReader = new StreamReader(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(streamReader.ReadToEnd());

            // set metadata
            DataHeader metadata = new DataHeader(saveData.metadata.version, saveData.metadata.date);

            DataHolder data = (DataHolder)ScriptableObject.CreateInstance<DataHolder>();
            data.OnCreateDataHolder(defaultData);

            // set data
            data.fileName = saveData.filename;
            data.originalTexture = TextureUtils.StringToTexture(saveData.originalTexture);
            data.originalColors = saveData.originalColors;

            List<ColorVariant> variantList = new List<ColorVariant>();
            variantList.AddRange(saveData.variants);
            data.colorVariants = variantList;

            data.variantButtonIndexStrings.Clear();
            data.variantColorEntryIndexStrings.Clear();

            streamReader.Close();
            streamReader.Dispose();

            return new ImportData(metadata, data);
        }
    }

    public struct ImportData
    {
        public DataHeader metadata;
        public DataHolder data;

        public ImportData(DataHeader metadata, DataHolder data)
        {
            this.metadata = metadata;
            this.data = data;
        }
    }
}
