using UnityEngine;

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

        public DataHolder(DataHolder defaultData) 
        {
            metaData = defaultData.metaData;
            fileName = defaultData.fileName;
            originalTexture = defaultData.originalTexture;
        }
    }
}
