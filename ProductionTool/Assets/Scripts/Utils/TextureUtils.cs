using UnityEngine;
using System.IO;
using UnityEngine.Rendering;

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
}
