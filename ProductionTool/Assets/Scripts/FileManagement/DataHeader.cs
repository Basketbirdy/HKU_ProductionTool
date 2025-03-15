using System;
using UnityEngine;

[System.Serializable]
public class DataHeader
{
    public string version;
    public string date;

    public DataHeader(string version, string date)
    {
        this.version = version;
        this.date = date;
    }
}
