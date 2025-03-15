using FileManagement;
using UnityEngine;

namespace FileManagement
{
    [System.Serializable]
    public struct WrappedData
    {
        public WrappedData(DataHeader header, DataHolder content)
        {
            this.header = header;
            this.content = content;
        }

        DataHeader header;
        DataHolder content;
    }
}
