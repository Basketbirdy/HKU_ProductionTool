using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

public static class TextureUtils
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
    public static Texture2D LoadImage(byte[] bytes)
    {
        if (bytes.Length != 0)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            texture.filterMode = FilterMode.Point;

            return texture;
        }
        else
        {
            return null;
        }
    }

    public static Texture2D GetShaderTexture(Texture2D inputTexture, Shader shader)
    {
        if (shader == null) { return null; }

        // create new material
        Material shaderMaterial = new Material(shader);
        shaderMaterial.SetTexture("_MainTex", inputTexture);

        // create Texture2D with material applied
        RenderTexture renderTexture = new RenderTexture(inputTexture.width, inputTexture.height, 0);
        Graphics.Blit(inputTexture, renderTexture, shaderMaterial);
        Texture2D processedTexture = new Texture2D(inputTexture.width, inputTexture.height);

        RenderTexture.active = renderTexture;
        processedTexture.ReadPixels(new Rect(0, 0, inputTexture.width, inputTexture.height), 0, 0);
        processedTexture.Apply();

        // Clean up the render texture
        RenderTexture.active = null;
        renderTexture.Release();

        return processedTexture;
    }

    public static Texture2D GetShaderTexture(Texture2D inputTexture, Material material)
    {
        if (material == null) { return null; }

        // create Texture2D with material applied
        RenderTexture renderTexture = new RenderTexture(inputTexture.width, inputTexture.height, 0);
        Graphics.Blit(inputTexture, renderTexture, material);

        Texture2D processedTexture = new Texture2D(inputTexture.width, inputTexture.height);
        RenderTexture.active = renderTexture;
        processedTexture.ReadPixels(new Rect(0, 0, inputTexture.width, inputTexture.height), 0, 0);
        processedTexture.Apply();

        // Clean up the render texture
        RenderTexture.active = null;
        renderTexture.Release();

        return processedTexture;
    }

    public static Texture2D CreateTextureSheet(Texture2D[] textures)
    {
        // determine amount of columns and rows to fit all textures inside a square
        int columns = Mathf.CeilToInt(Mathf.Sqrt(textures.Length));
        int rows = Mathf.CeilToInt((float)textures.Length / columns);

        // determine individual texture dimensions
        // every texture should have the same dimensions
        int textureWidth = textures[0].width;
        int textureHeight = textures[0].height;

        // calculate spritesheet dimensions (no padding)
        int sheetWidth = columns * textureWidth;
        int sheetHeight = rows * textureHeight;

        // create sprite sheet texture
        Texture2D sheetTexture = new Texture2D(sheetWidth, sheetHeight);
        sheetTexture.filterMode = FilterMode.Point;
        sheetTexture.filterMode = FilterMode.Point;

        // make texture fully transparent
        Color transparent = new Color(0,0,0,0);
        Color[] pixels = new Color[sheetTexture.width * sheetTexture.height];
        for (var i = 0; i < pixels.Length; i++)
        {
            pixels[i] = transparent;
        }
        sheetTexture.SetPixels(pixels);
        sheetTexture.Apply();

        // allign textures in sheet
        for (int i = 0; i < textures.Length; i++)
        {
            int columnPos = i % columns;
            int rowPos = i / columns;

            int xPos = columnPos * textureWidth;
            int yPos = rowPos * textureHeight;

            // Ensure we're within bounds
            if (xPos + textureWidth <= sheetWidth && yPos + textureHeight <= sheetHeight)
            {
                // Copy the pixels from the current texture to the correct position in the spritesheet
                sheetTexture.SetPixels(xPos, yPos, textureWidth, textureHeight, textures[i].GetPixels());
            }
            else
            {
                Debug.LogError($"Error: Texture {i} exceeds spritesheet bounds.");
            }
        }

        sheetTexture.Apply();

        return sheetTexture;
    }

    public static Texture2D CreateColorTexture(params Color[][] colors)
    {
        if (colors.Length == 0) return null;

        int width = 256;
        int height = colors.Length;

        Texture2D texture = new Texture2D(width, height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if(j >= colors[i].Length) { texture.SetPixel(j, i, new Color(0, 0, 0, 0)); }
                else { texture.SetPixel(j, i, colors[i][j]); }
            }
        }
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return texture;
    }

    public static Color[] GetUniqueColors(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();

        HashSet<Color> colors = new HashSet<Color>(); // hashset because duplicates get ignored

        for (int i = 0; i < pixels.Length; i++)
        {
            colors.Add(pixels[i]);
        }

        return colors.ToArray(); // returns an array so the indexes are known
    }
}
