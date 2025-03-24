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

        [Header("User interface")]
        [SerializeField] private UserInterfaceIdentifiers userInterfaceIdentifiers;
        private ToolUserInterfaceHandler toolUserInterfaceHandler;

        [Header("Importing")]
        private ImportHandler importHandler;

        [Header("Project")]
        [SerializeField] private DataHeader currentMetadata;
        [SerializeField] private DataHolder currentData;
        private DataHandler dataHandler;

        [SerializeField] private ColorPicker colorPicker;
        [SerializeField] private int maxColors = 256;

        [Header("ColorVariants")]
        [SerializeField] private VisualTreeAsset colorEntryTemplate;
        [SerializeField] private VisualTreeAsset variantButtonTemplate;


        [Header("Shader")]
        [SerializeField] private Shader shader;
        private Material shaderMaterial;
        [SerializeField] private Color oldColor;
        [SerializeField] private Color newColor;

        [Header("Saving")]
        [SerializeField] private string currentSavePath;
        private SaveHandler saveHandler;

        [Header("Exporting")]
        private ExportHandler exportHandler;

        [Header("Events")]
        public static Action onColorDataChange;

        // base monobehaviour -------------------------------------------------
        private void Awake()
        {
            importHandler = new ImportHandler();
            dataHandler = new DataHandler(defaultData);
            saveHandler = new SaveHandler();
            exportHandler = new ExportHandler();
            toolUserInterfaceHandler = new ToolUserInterfaceHandler(userInterfaceIdentifiers, variantButtonTemplate, colorEntryTemplate);

            shaderMaterial = new Material(shader);
        }

        private void Start()
        {
            colorPicker.Initialize();

            // create ui toolkit references
            toolUserInterfaceHandler.InitializeReferences();
            // add listeners to buttons
            toolUserInterfaceHandler.InitializeButtons(
                OnImportButtonPressed,
                OnExportButtonPressed,
                OnSaveButtonPressed,
                OnSaveAsButtonPressed,
                OnAddVariantButtonClicked,
                OnRemoveVariantButtonClicked,
                OnExportContentChange,
                OnExportFiletypeChange
                );

            // events
            onColorDataChange += OnColorDataChange;
        }

        private void OnDisable()
        {
            // remove any button listeners
            toolUserInterfaceHandler.OnDisable(currentData.originalColors.Length,
                OnImportButtonPressed,
                OnExportButtonPressed,
                OnSaveButtonPressed,
                OnSaveAsButtonPressed,
                OnAddVariantButtonClicked,
                OnRemoveVariantButtonClicked,
                OnExportContentChange,
                OnExportFiletypeChange
                );

            // events
            onColorDataChange -= OnColorDataChange;
        }

        // main button events ----------------------------------------
        private void OnImportButtonPressed()
        {
            if(currentData != null) { /* ask for confirmation */ }

            // select file
            var extensions = new[] {
                new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
                new ExtensionFilter("Project Files", "cosw")
            };
            var url = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (url.Length == 0) { Debug.LogWarning($"No url selected"); return; }
            Debug.Log($"Path selected: {url[0]}; File extension: {Path.GetExtension(url[0])}");

            // if there is data, clear the dynamic user interface (variant buttons & color entries)
            if (currentData != null) { toolUserInterfaceHandler.ClearDynamicUserInterface(currentData.originalColors.Length , currentData.colorVariants.Count); }

            // check file extension
            string extension = Path.GetExtension(url[0]);
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                // if imported file is an image, create new project data
                currentMetadata = dataHandler.CreateMetadata(CurrentVersion);
                currentData = dataHandler.CreateFreshProject(url[0]);
                currentSavePath = "";

                if(currentData.originalColors.Length > maxColors)
                {
                    Debug.LogError("Too many different colors! abandoning import");
                    currentData = null;
                    return;
                }
            }
            else if(extension == ".cosw")
            {
                // if imported file is a save, use imported data
                ImportData import = importHandler.ImportExistingProject(url[0], defaultData);
                if(import.metadata == null || import.data == null) { Debug.LogWarning("Importing data failed! TODO - error screen"); return; }
                currentMetadata = import.metadata;
                currentData = import.data;
                currentSavePath = url[0];
            }
            else
            {
                Debug.LogWarning("Unsupported file type!");
                return;
            }

            UserInterfaceHandler.instance.AssignVisualElementBackground(userInterfaceIdentifiers.originalSpriteId, currentData.originalTexture);
            UserInterfaceHandler.instance.SetLabel(userInterfaceIdentifiers.filenameLabelId, currentData.fileName);

            EvaluateColorVariants();
            EvaluateColorEntries();

            toolUserInterfaceHandler.UpdateProcessedImage(currentData, shaderMaterial);
        }
        private void OnExportButtonPressed()
        {
            // format a default file name
            string currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string defaultName = "ColorVariants" + "_" + currentTime;

            // get url to export to
            string url = GetSavePath("Export file", Application.persistentDataPath, defaultName, null);
            if(url == "") { return; }
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
        private void OnSaveButtonPressed()
        {
            if(currentData == null) { return; }
            if(currentSavePath == null || currentSavePath == "") { OnSaveAsButtonPressed(); return; }

            saveHandler.Save(currentSavePath, currentData, CurrentVersion);
        }
        private void OnSaveAsButtonPressed()
        {
            if(currentData == null) { return; }

            var extensions = new[]
            {
                new ExtensionFilter("Project files", "cosw")
            };

            string path = GetSavePath("Save file", Application.persistentDataPath, "New_ColorVariations", extensions);
            if(path == "") { return; }

            saveHandler.Save(path, currentData, CurrentVersion);
            currentSavePath = path;
        }

        // file path -------------------------------------------------
        private string GetSavePath(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            var url = StandaloneFileBrowser.SaveFilePanel("Open File", directory, defaultName, extensions);
            if (url.Length == 0) { Debug.LogWarning($"No url selected"); return null; }
            Debug.Log($"Path selected: {url}");
            return url;
        }

        // exporting -------------------------------------------------
        private void OnExportContentChange(ChangeEvent<string> evt)
        {
            if(currentData == null) { return; }

            int selectedIndex = UserInterfaceHandler.instance.GetDropdownValue(userInterfaceIdentifiers.exportContentDropdownId);

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

            int selectedIndex = UserInterfaceHandler.instance.GetDropdownValue(userInterfaceIdentifiers.exportFiletypeDropdownId);

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
                Texture2D outputTexture = TextureUtils.CreateColorTexture1D(newColors);
                tempMaterial.SetTexture("_OutputColors", outputTexture);

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

        // static user interface ------------------------------------
        private void OnColorDataChange()
        {
            toolUserInterfaceHandler.UpdateColorEntryButtons(currentData.originalColors.Length, currentData.colorVariants[currentData.selectedIndex].newColors);
            toolUserInterfaceHandler.UpdateProcessedImage(currentData, shaderMaterial);
        }

        // dynamic ui ------------------------------------------------
        private void EvaluateColorEntries()
        {
            for(int i = 0; i < currentData.originalColors.Length; i++)
            {
                //if(i == 0) { continue; }

                // Create ui from template
                int colorIndex = i;
                string desiredKey = userInterfaceIdentifiers.colorEntryDefaultId + i;
                currentData.variantColorEntryIndexStrings.Add(colorIndex, desiredKey);
                toolUserInterfaceHandler.CreateColorEntry(colorIndex, desiredKey, currentData.originalColors[colorIndex], OnChangeColorButtonClicked);
            }
        }
        private void EvaluateColorVariants()
        {
            for(int i = 0; i < currentData.colorVariants.Count; i++)
            {
                int variantIndex = i;
                string desiredKey = userInterfaceIdentifiers.variantButtonDefaultId + variantIndex;
                currentData.variantButtonIndexStrings.Add(variantIndex, desiredKey);
                toolUserInterfaceHandler.CreateVariantButton(variantIndex, desiredKey, OnVariantButtonClicked);
            }
        }

        // color variants
        private void OnAddVariantButtonClicked()
        {
            if(currentData == null) { return; }

            // handle ui
            Debug.Log("Add variant clicked!");
            int variantIndex = currentData.colorVariants.Count;
            string desiredKey = userInterfaceIdentifiers.variantButtonDefaultId + variantIndex;
            toolUserInterfaceHandler.CreateVariantButton(variantIndex, desiredKey, OnVariantButtonClicked);

            // create new variant
            currentData.colorVariants.Add(new ColorVariant(desiredKey, currentData.originalColors));
        }
        private void OnRemoveVariantButtonClicked()
        {
            Debug.Log("Remove variant clicked!");
 
        }
        private void OnVariantButtonClicked(int index)
        {
            SelectVariant(index);
        }
        private void SelectVariant(int index)
        {
            currentData.selectedIndex = index;
            onColorDataChange?.Invoke();
        }

        // color picker
        private void OnChangeColorButtonClicked(int index)
        {
            Debug.Log($"Clicked color index button: {index}");
            if (colorPicker.GetState()) { colorPicker.OnCancelButtonClicked(); }

            colorPicker.Open(currentData.colorVariants[currentData.selectedIndex].newColors[index], currentData.originalColors[index], index);
            colorPicker.AddColorListener(OnColorPickerClosed);
        }
        private void OnColorPickerClosed(Color32 color, int index)
        {
            colorPicker.RemoveColorListener(OnColorPickerClosed);
            if(color == new Color(0, 0, 0, 0)) { return; }

            Debug.Log($"Color picked! color: {color}");

            // change color data
            currentData.colorVariants[currentData.selectedIndex].newColors[index] = color;
            onColorDataChange?.Invoke();
        }
        
        // custom editor buttons ------------------------------------
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
            Debug.Log($"version: {currentMetadata.version}");
            Debug.Log($"Date: {currentMetadata.date}");
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
