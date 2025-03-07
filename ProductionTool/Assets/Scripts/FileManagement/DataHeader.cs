using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDataHeader", menuName = "ScriptableObjects/DataHeader")]
public class DataHeader : ScriptableObject
{
    public string version;
    public string date;

    public DataHeader(DataHeader defaultDataHeader)
    {
        version = defaultDataHeader.version;
        date = DateTime.Now.ToString();
    }
}
