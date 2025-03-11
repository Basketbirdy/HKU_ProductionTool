using SFB;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FileManagement
{
    public class FileManager : MonoBehaviour
    {
        [Header("MetaData")]
        [SerializeField] private DataHolder defaultData;
        [HideInInspector] [SerializeField] private int majorVersion; // Serializing the field stops the int from resetting in playmode,
        [HideInInspector] [SerializeField] private int minorVersion;    // allowing me to change it in editor with the custom editor
        [HideInInspector] [SerializeField] private int patchVersion;
        public string CurrentVersion => string.Join(".", majorVersion, minorVersion, patchVersion);

        [Header("Importing")]
        [SerializeField] private string importButtonId;
        private ImportHandler importHandler;

        [Header("Project")]
        private DataHandler dataHandler;
        [SerializeField] private DataHolder currentData;
        [Space]
        [SerializeField] private string addVariantButtonid = "Button_AddVariant";
        [SerializeField] private string removeVariantButtonid = "Button_RemoveVariant";
        [SerializeField] private string variantButtonId = "Button_Variant";
        [SerializeField] private string variantButtonAreaId = "VariantContainerMask";
        [SerializeField] private VisualTreeAsset variantButtonTemplate;

        [Header("Shader")]
        [SerializeField] private Shader shader;
        private Material shaderMaterial;
        [SerializeField] private Color oldColor;
        [SerializeField] private Color newColor;

        [Header("Saving")]
        private SaveHandler saveHandler;

        [Header("Exporting")]
        [SerializeField] private string exportButtonId = "Button_Export";
        [SerializeField] private string exportContentDropdownId = "Dropdown_ExportContent";
        [SerializeField] private string exportFiletypeDropdownId = "Dropdown_ExportFiletype";
        private ExportHandler exportHandler;

        [Header("Dynamic User Interface Ids")]
        [SerializeField] private string originalSpriteId = "Sprite_Original";
        [SerializeField] private string processedSpriteId = "Sprite_Processed";
        [SerializeField] private string filenameLabelId = "Label_Filename";


        private void Awake()
        {
            importHandler = new ImportHandler();
            dataHandler = new DataHandler(defaultData);
            saveHandler = new SaveHandler();
            exportHandler = new ExportHandler();

            shaderMaterial = new Material(shader);
        }

        private void Start()
        {
            // static ui
                // buttons
            UserInterfaceHandler.instance.AddButtonRef(importButtonId);
            UserInterfaceHandler.instance.AddButtonListener(importButtonId, OnImportButtonPressed);

            UserInterfaceHandler.instance.AddButtonRef(exportButtonId);
            UserInterfaceHandler.instance.AddButtonListener(exportButtonId, OnExportButtonPressed);

            UserInterfaceHandler.instance.AddButtonRef(addVariantButtonid);
            UserInterfaceHandler.instance.AddButtonListener(addVariantButtonid, OnAddVariantButtonClicked);
            UserInterfaceHandler.instance.AddButtonRef(removeVariantButtonid);
            UserInterfaceHandler.instance.AddButtonListener(removeVariantButtonid, OnRemoveVariantButtonClicked);
            UserInterfaceHandler.instance.AddButtonRef(variantButtonId);
                // dropdowns
            UserInterfaceHandler.instance.AddDropdownRef(exportContentDropdownId);
            UserInterfaceHandler.instance.AddDropdownListener(exportContentDropdownId, OnExportContentChange);
            UserInterfaceHandler.instance.AddDropdownRef(exportFiletypeDropdownId);
            UserInterfaceHandler.instance.AddDropdownListener(exportFiletypeDropdownId, OnExportFiletypeChange);

            // dynamic ui
                // visual elements - images
            UserInterfaceHandler.instance.AddVisualElementRef(originalSpriteId);
            UserInterfaceHandler.instance.AddVisualElementRef(processedSpriteId);

            UserInterfaceHandler.instance.AddVisualElementRef(variantButtonAreaId);
                // labels - text
            UserInterfaceHandler.instance.AddLabelRef(filenameLabelId);

            shaderMaterial.SetColor("_OldColor", oldColor);
            shaderMaterial.SetColor("_NewColor", newColor);
        }

        private void OnDisable()
        {
            // static ui
                // buttons
            UserInterfaceHandler.instance.RemoveButtonListener(importButtonId, OnImportButtonPressed);
            UserInterfaceHandler.instance.RemoveButtonListener(exportButtonId, OnExportButtonPressed);

            UserInterfaceHandler.instance.RemoveButtonListener(addVariantButtonid, OnAddVariantButtonClicked);
            UserInterfaceHandler.instance.RemoveButtonListener(removeVariantButtonid, OnRemoveVariantButtonClicked);

            // dropdowns
            UserInterfaceHandler.instance.RemoveDropdownListener(exportContentDropdownId, OnExportContentChange);
            UserInterfaceHandler.instance.RemoveDropdownListener(exportFiletypeDropdownId, OnExportFiletypeChange);
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

            shaderMaterial.SetColorArray("oldColors", currentData.originalColors);
            UpdateUserInterface(currentData);
        }

        private void OnSaveButtonPressed()
        {

        }

        private void OnExportButtonPressed()
        {
            // format a default file name
            string currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string defaultName = "ColorVariants" + "_" + currentTime;

            // get url to export to
            var url = StandaloneFileBrowser.SaveFilePanel("Open File", Application.persistentDataPath, defaultName, "");
            if (url.Length == 0) { Debug.LogWarning($"No url selected"); return; }
            if (File.Exists(url)) { Debug.LogWarning($"File already exists at path! cancelling export"); return; }
            Debug.Log($"Path selected: {url}");

            // get the textures to export 
            ExportOptions exportOptions = currentData.exportOptions;
            Debug.Log($"Options; content: {exportOptions.content}, fileType: {exportOptions.fileType}");

            Texture2D[] texturesToExport = null;
            switch (exportOptions.content)
            {
                case ExportContent.SELECTED:
                    texturesToExport = GetSelectedTexture();
                    break;
                case ExportContent.ALL: 
                    texturesToExport = GetAllTextures();
                    break;
                case ExportContent.SPRITESHEET: 
                    texturesToExport = GetSpriteSheetTexture();
                    break;
            }

            exportHandler.Export(url, texturesToExport, exportOptions.fileType);
        }

        private void OnAddVariantButtonClicked()
        {
            Debug.Log("Add variant clicked!");
            UserInterfaceHandler.instance.InsertTemplateContainerIntoVisualElement(variantButtonAreaId, variantButtonTemplate);

        }
        private void OnRemoveVariantButtonClicked()
        {
            Debug.Log("Remove variant clicked!");

        }

        private void OnExportContentChange(ChangeEvent<string> evt)
        {
            if(currentData == null) { return; }

            int selectedIndex = UserInterfaceHandler.instance.GetDropdownValue(exportContentDropdownId);

            switch(selectedIndex)
            {
                case 0: // selected
                    currentData.exportOptions.content = ExportContent.SELECTED;
                    break;
                case 1: // all
                    currentData.exportOptions.content = ExportContent.ALL;
                    break;
                case 2: // spritesheet
                    currentData.exportOptions.content = ExportContent.SPRITESHEET;
                    break;
            }

            Debug.Log($"CurrentData export settings; content: {currentData.exportOptions.content}, filetype: {currentData.exportOptions.fileType}");
        }

        private void OnExportFiletypeChange(ChangeEvent<string> evt)
        {
            if (currentData == null) { return; }

            int selectedIndex = UserInterfaceHandler.instance.GetDropdownValue(exportFiletypeDropdownId);

            switch (selectedIndex)
            {
                case 0: // png
                    currentData.exportOptions.fileType = FileType.PNG;
                    break;
                case 1: // jpg
                    currentData.exportOptions.fileType = FileType.JPG;
                    break;
            }

            Debug.Log($"CurrentData export settings; content: {currentData.exportOptions.content}, filetype: {currentData.exportOptions.fileType}");
        }

        private Texture2D[] GetSelectedTexture()
        {
            // set the new colors of the shader material to the colors saved in the variant
            Color[] newColors = currentData.colorVariants[currentData.selectedIndex].newColors;
            Material tempMaterial = shaderMaterial;
            tempMaterial.SetColorArray("_newColors", newColors);
            Texture2D texture = TextureUtils.GetShaderTexture(currentData.originalTexture, tempMaterial);
            return new Texture2D[1] { texture };
        }

        private Texture2D[] GetAllTextures()
        {
            Texture2D[] textures = new Texture2D[currentData.colorVariants.Count];
            for(int i = 0; i < currentData.colorVariants.Count; i++)
            {
                Color[] newColors = currentData.colorVariants[i].newColors;
                Material tempMaterial = shaderMaterial;
                tempMaterial.SetColorArray("_newColors", newColors);
                textures[i] = TextureUtils.GetShaderTexture(currentData.originalTexture, tempMaterial);
            }
            return textures;
        }

        private Texture2D[] GetSpriteSheetTexture()
        {
            // get all textures for in the spritesheet
            Texture2D[] textures = GetAllTextures();

            return new Texture2D[1] { TextureUtils.CreateTextureSheet(textures) };
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
        public void PrintData()
        {
            if(currentData == null) { return; }

            Debug.Log($"Filename: {currentData.fileName}");
            Debug.Log($"-----------Metadata-------------");
            Debug.Log($"version: {currentData.metaData.version}");
            Debug.Log($"Date: {currentData.metaData.date}");
            Debug.Log($"--------------------------------");

            int count = 0;
            foreach(Color color in currentData.originalColors)
            {
                Debug.Log($"[{count}] {color}");
                count++;
            }
        }
    }
}
