using UnityEngine;
using System.Collections.Generic;

namespace FileManagement
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewData", menuName = "ScriptableObjects/Data")]
    public class DataHolder : ScriptableObject
    {
        // meta data
        public DataHeader metaData;

        [Space]

        [Header("Instance data")]
        public string fileName;
        public Texture2D originalTexture;
        public Color[] originalColors;

        // variants
        public List<ColorVariant> colorVariants = new List<ColorVariant>();

        [Header("State data")]
        public int selectedIndex;

        // options
        public ExportOptions exportOptions;

        public DataHolder(DataHolder defaultData) 
        {
            metaData = defaultData.metaData;
            fileName = defaultData.fileName;
            originalTexture = defaultData.originalTexture;

            colorVariants = defaultData.colorVariants;
            exportOptions = defaultData.exportOptions;
        }

        public void OnCreateDataHolder(DataHolder defaultData)
        {
            metaData = defaultData.metaData;
            fileName = defaultData.fileName;
            originalTexture = defaultData.originalTexture;
        }
    }
}
