using UnityEngine;

namespace FileManagement
{
    [System.Serializable]
    public class SaveData
    {
        public DataHeader metadata;

        public string filename;
        public string originalTexture;
        public Color[] originalColors;
        public ColorVariant[] variants; // contains variant name and array of colors

        public void Initialize(DataHeader metadata, string filename, string originalTexture, Color[] originalColors, ColorVariant[] variants)
        {
            this.metadata = metadata;

            this.filename = filename;
            this.originalTexture = originalTexture;
            this.originalColors = originalColors;
            this.variants = variants;
        }
    }
}
