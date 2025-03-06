using UnityEngine;
using System.IO;

public static class FileUtils
{
    public static Texture2D LoadImage(string path)
    {
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2,2);
            texture.LoadImage(bytes);
            texture.filterMode = FilterMode.Point;

            return texture;
        }
        else
        {
            return null;
        }
    }   
}
