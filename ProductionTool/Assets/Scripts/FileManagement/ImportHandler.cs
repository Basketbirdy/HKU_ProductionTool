using UnityEngine;
using System.Collections.Generic;
using SFB;
using System.Windows.Forms;
using Unity.VisualScripting;
using System.IO;

public class ImportHandler
{
    private string importButtonId;

    // temp
    private Material shaderMaterial;
    private Shader shader;

    public ImportHandler(string importButtonId, Material shaderMaterial, Shader shader)
    {
        this.importButtonId = importButtonId;

        this.shaderMaterial = shaderMaterial;
        this.shader = shader;

        UserInterfaceHandler.instance.AddButtonRef(importButtonId);
        UserInterfaceHandler.instance.AddButtonListener(importButtonId, OnImport);

        // TODO - get this somewhere less coupled 
        UserInterfaceHandler.instance.AddVisualElementRef("OriginalSprite");
        UserInterfaceHandler.instance.AddVisualElementRef("UpdatedSprite");
        UserInterfaceHandler.instance.AddLabelRef("Label_Filename");
    }

    public void OnDisable()
    {
        UserInterfaceHandler.instance.RemoveButtonListener(importButtonId, OnImport);
    }

    private void OnImport()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
            new ExtensionFilter("Project Files", "cosw")
        };
        var url = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (url.Length == 0) { Debug.LogWarning($"No url selected"); return; }
        Debug.Log($"Path selected: {url[0]}; File extension: {Path.GetExtension(url[0])}");
        string extension = Path.GetExtension(url[0]);
        if(extension == ".png" || extension == ".jpg" || extension == ".jpeg")
        {
            CreateProject(url[0]);
        }
        else
        {
            LoadProject(url[0]);
        }
    }

    private void LoadProject(string url)
    {
        Debug.LogWarning("Loading not implemented yet!");
    }

    private void CreateProject(string url)
    {
        Debug.LogWarning("Creating new project not implemented yet!");
        // TODO - Create a project and update ui somewhere else based on updated project data
        // get data
        Texture2D texture = TextureUtils.LoadImage(url);
        Texture2D shaderTexture = TextureUtils.GetShaderTexture(texture, shaderMaterial);
        shaderTexture.filterMode = FilterMode.Point;

        // update user interface
        UserInterfaceHandler.instance.AssignVisualElementBackground("OriginalSprite", texture);
        UserInterfaceHandler.instance.AssignVisualElementBackground("UpdatedSprite", shaderTexture);

        string fileName = Path.GetFileName(url);
        UserInterfaceHandler.instance.SetLabel("Label_Filename", fileName);
    }
}
