using SFB;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace FileManagement
{
    public class FileManager : MonoBehaviour
    {
        [Header("MetaData")]
        [SerializeField] private DataHolder defaultData;
        private int majorVersion = 0;
        private int minorVersion = 0;
        private int patchVersion = 0;
        public string CurrentVersion => string.Join(".", majorVersion, minorVersion, patchVersion);

        [Header("Importing")]
        [SerializeField] private string importButtonId;
        private ImportHandler importHandler;

        [Header("Project")]
        private DataHandler dataHandler;
        [SerializeField] private DataHolder currentData;

        [Header("Shader")]
        [SerializeField] private Shader shader;
        private Material shaderMaterial;
        [SerializeField] private Color oldColor;
        [SerializeField] private Color newColor;

        [Header("Saving")]
        private SaveHandler saveHandler;

        [Header("Exporting")]
        private ExportHandler exportHandler;

        [Header("User Interface Ids")]
        [SerializeField] private string originalSpriteId = "Sprite_Original";
        [SerializeField] private string processedSpriteId = "Sprite_Processed";
        [SerializeField] private string filenameLabelId = "Label_Filename";

        private void Awake()
        {
            importHandler = new ImportHandler(importButtonId, shaderMaterial, shader);
            dataHandler = new DataHandler(defaultData);
            saveHandler = new SaveHandler();
            exportHandler = new ExportHandler();

            shaderMaterial = new Material(shader);
        }

        private void Start()
        {
            UserInterfaceHandler.instance.AddButtonRef(importButtonId);
            UserInterfaceHandler.instance.AddButtonListener(importButtonId, OnImportButtonPressed);

            UserInterfaceHandler.instance.AddVisualElementRef(originalSpriteId);
            UserInterfaceHandler.instance.AddVisualElementRef(processedSpriteId);
            UserInterfaceHandler.instance.AddLabelRef(filenameLabelId);

            shaderMaterial.SetColor("_OldColor", oldColor);
            shaderMaterial.SetColor("_NewColor", newColor);
        }

        private void OnDisable()
        {
            UserInterfaceHandler.instance.RemoveButtonListener(importButtonId, OnImportButtonPressed);
        }

        private void OnImportButtonPressed()
        {
            // select file
            var extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                new ExtensionFilter("Project Files", "cosw")
            };
            var url = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (url.Length == 0) { Debug.LogWarning($"No url selected"); return; }
            Debug.Log($"Path selected: {url[0]}; File extension: {Path.GetExtension(url[0])}");

            // check file extension
            string extension = Path.GetExtension(url[0]);
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                currentData = dataHandler.CreateFreshProject(url[0], CurrentVersion);
            }
            else if(extension == ".cosw")
            {
                currentData = importHandler.ImportExistingProject(url[0]);
            }
            else
            {
                Debug.LogWarning("Unsupported file type!");
                return;
            }

            UpdateUserInterface(currentData);
        }

        private void OnSaveButtonPressed()
        {

        }

        private void OnExportButtonPressed()
        {

        }


        private void UpdateUserInterface(DataHolder data)
        {
            UserInterfaceHandler.instance.AssignVisualElementBackground(originalSpriteId, currentData.originalTexture);

            Texture2D processedTexture = TextureUtils.GetShaderTexture(data.originalTexture, shaderMaterial);
            processedTexture.filterMode = FilterMode.Point;
            UserInterfaceHandler.instance.AssignVisualElementBackground(processedSpriteId, processedTexture);

            UserInterfaceHandler.instance.SetLabel(filenameLabelId , data.fileName);
        }

        // editor button methods
        public void IncrementMajorVersion() { majorVersion++; }
        public void DecrementMajorVersion() { majorVersion--; }
        public void IncrementMinorVersion() { minorVersion++; }
        public void DecrementMinorVersion() { minorVersion--; }
        public void IncrementPatchVersion() { patchVersion++; }
        public void DecrementPatchVersion() { patchVersion--; }
    }
}
