using UnityEngine;

public class FileManager : MonoBehaviour
{
    [Header("Importing")]
    [SerializeField] private string importButtonId;
    private ImportHandler importHandler;

    [Header("Project")]
    private DataHandler dataHandler;

    //[Header("Saving")]
    //[SerializeField] private string tempStr0;

    //[Header("Exporting")]
    //[SerializeField] private string tempStr1;

    private void Start()
    {
        importHandler = new ImportHandler(importButtonId);
        dataHandler = new DataHandler();
    }

    private void OnDisable()
    {
        if (importHandler != null) { importHandler.OnDisable(); }
    }
}
