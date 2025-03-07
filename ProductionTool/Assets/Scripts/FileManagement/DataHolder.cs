using UnityEngine;

namespace FileManagement
{
    public class DataHolder
    {
        private HeaderData metaData;
        private HeaderData defaultData;

        private Texture2D originalTexture;
    }


    [CreateAssetMenu(fileName = "NewHeaderData", menuName = "ScriptableObjects/HeaderData")]
    public class HeaderData : ScriptableObject
    {
        public string version;

        public HeaderData(string version)
        {
            this.version = version;
        }
    }
}
