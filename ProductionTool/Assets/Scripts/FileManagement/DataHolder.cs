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

        // instance data
        public string fileName;
        public Texture2D originalTexture;
        public HashSet<Color> originalColors = new HashSet<Color>();

        public DataHolder(DataHolder defaultData) 
        {
            metaData = defaultData.metaData;
            fileName = defaultData.fileName;
            originalTexture = defaultData.originalTexture;
        }

        public void OnCreateDataHolder(DataHolder defaultData)
        {
            metaData = defaultData.metaData;
            fileName = defaultData.fileName;
            originalTexture = defaultData.originalTexture;
        }
    }
}
