using UnityEngine;

public class FileManager : MonoBehaviour
{
    [Header("Importing")]
    [SerializeField] private string importButtonId;
    private ImportHandler importHandler;

    [Header("Project")]
    private DataHandler dataHandler;

    [Header("Shader")]
    [SerializeField] private Shader shader;
    private Material shaderMaterial;
    [SerializeField] private Color oldColor;
    [SerializeField] private Color newColor;

    //[Header("Saving")]
    //[SerializeField] private string tempStr0;

    //[Header("Exporting")]
    //[SerializeField] private string tempStr1;

    private void Start()
    {
        shaderMaterial = new Material(shader);
        shaderMaterial.SetColor("_OldColor", oldColor);
        shaderMaterial.SetColor("_NewColor", newColor);

        importHandler = new ImportHandler(importButtonId, shaderMaterial, shader);
        dataHandler = new DataHandler();
    }

    private void OnDisable()
    {
        if (importHandler != null) { importHandler.OnDisable(); }
    }
}
