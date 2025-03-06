using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Header("Importing")]
    [SerializeField] private string importButtonId;
    private ImportHandler importHandler;

    //[Header("Saving")]
    //[SerializeField] private string tempStr0;

    //[Header("Exporting")]
    //[SerializeField] private string tempStr1;

    private void Awake()
    {
    }

    private void Start()
    {
        importHandler = new ImportHandler(importButtonId);
    }

    private void OnDisable()
    {
        importHandler.OnDisable();
    }
}
