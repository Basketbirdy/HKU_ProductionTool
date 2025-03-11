using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ColorVariant
{
    public string name;
    public Color[] newColors;

    public ColorVariant(string name, Color[] originalColors)
    {
        this.name = name;
        newColors = new Color[originalColors.Length];

        for(int i = 0; i < newColors.Length; i++)
        {
            newColors[i] = originalColors[i];
        }
    }

    public void SetColorAtIndex(int index, Color color)
    {
        newColors[index] = color;
    }
}
