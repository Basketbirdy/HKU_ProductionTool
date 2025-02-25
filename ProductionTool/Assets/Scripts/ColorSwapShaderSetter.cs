using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
struct ColorPair
{
    public Color[] oldColors;
    public Color[] newColors;
}

public class ColorSwapShaderSetter : MonoBehaviour
{
    private Material material;
    [SerializeField] private ColorPair[] colorPairs;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().GetComponent<Material>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < colorPairs.Length; i++)
        {
            //material.SetColorArray("_OldColors", colorPairs[i].oldColors);
            //material.SetColorArray("_NewColors", colorPairs[i].newColors);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
