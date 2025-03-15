using UnityEngine;
using System.Collections.Generic;

namespace FileManagement
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NewData", menuName = "ScriptableObjects/Data")]
    public class DataHolder : ScriptableObject
    {
        [Header("Instance data")]
        public string fileName;
        public Texture2D originalTexture;
        public Color[] originalColors;

        // variants
        public List<ColorVariant> colorVariants = new List<ColorVariant>();

        [Header("State data")]
        public int selectedIndex;
        public Dictionary<int, string> variantButtonIndexStrings = new Dictionary<int, string>();
        public Dictionary<int, string> variantColorEntryIndexStrings = new Dictionary<int, string>();

        // options
        public ExportOptions exportOptions;

        public DataHolder(DataHolder defaultData) 
        {
            fileName = defaultData.fileName;
            originalTexture = defaultData.originalTexture;

            colorVariants = defaultData.colorVariants;
            exportOptions = defaultData.exportOptions;

            variantButtonIndexStrings = new Dictionary<int, string>();
            variantColorEntryIndexStrings = new Dictionary<int, string>();
        }

        public void OnCreateDataHolder(DataHolder defaultData)
        {
            fileName = defaultData.fileName;
            originalTexture = defaultData.originalTexture;
            colorVariants = defaultData.colorVariants;

            selectedIndex = defaultData.selectedIndex;
            exportOptions = defaultData.exportOptions;

            variantButtonIndexStrings = new Dictionary<int, string>();
            variantColorEntryIndexStrings = new Dictionary<int, string>();
        }
    }
}
