using UnityEngine;
using System.Collections.Generic;
using SFB;
using System.Windows.Forms;
using Unity.VisualScripting;

public class ImportHandler
{
    private string importButtonId;

    public ImportHandler(string importButtonId)
    {
        this.importButtonId = importButtonId;
        UserInterfaceHandler.instance.AddButtonListener(importButtonId, OnImport);
    }

    public void OnDisable()
    {
        UserInterfaceHandler.instance.RemoveButtonListener(importButtonId, OnImport);
    }

    private void OnImport()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
            new ExtensionFilter(".cosw Files", "cosw")
        };
        var url = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (url.Length == 0) { Debug.LogWarning($"No url selected"); return; }
        Debug.Log($"Path selected: {url}");
    }
}
