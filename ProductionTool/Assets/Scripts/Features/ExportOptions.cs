using UnityEngine;

namespace FileManagement
{
    public enum FileType { PNG, JPG }
    public enum ExportContent { SELECTED, ALL, SPRITESHEET }
    [System.Serializable]
    public struct ExportOptions
    {
        public ExportContent content;
        public FileType fileType;
    }
}
