using SFB;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor.VersionControl;

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
        [SerializeField] private ColorPicker colorPicker;

        [Header("ColorVariants")]
        [SerializeField] private string colorEntryScrollViewId = "ScrollView_ColorContainer";
        [SerializeField] private string colorEntryDefaultId = "Element_VariantColorEntry";
        [SerializeField] private string colorEntryOriginalColorDefaultId = "Element_OriginalColor";
        [SerializeField] private string colorEntryIndexLabelDefaultId = "Label_VariantIndex";
        [SerializeField] private string colorEntryNewColorButtonDefaultId = "Button_NewColor";
        [SerializeField] private VisualTreeAsset colorEntryTemplate;
        [Space]
        [SerializeField] private string addVariantButtonid = "Button_AddVariant";
        [SerializeField] private string removeVariantButtonid = "Button_RemoveVariant";
        [SerializeField] private string variantButtonId = "Button_Variant";
        [SerializeField] private string variantButtonAreaId = "VariantContainerMask";
        [SerializeField] private string variantButtonDefaultId = "Button_Variant";
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
            colorPicker.Initialize();

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

            // scrollviews
            UserInterfaceHandler.instance.AddScrollViewRef(colorEntryScrollViewId);

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

            // dynamic ui
            for(int i = 0; i < currentData.originalColors.Length; i++)
            {
                UserInterfaceHandler.instance.RemoveButtonListener<int>(colorEntryNewColorButtonDefaultId + i);
            }
        }

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

            ClearDynamicUserInterface();

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
            CreateVariantUI();
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
            if(currentData == null) { return; }

            // handle ui
            Debug.Log("Add variant clicked!");
            int variantIndex = currentData.colorVariants.Count;
            string desiredKey = variantButtonDefaultId + variantIndex;
            currentData.variantIndexStrings.Add(variantIndex, desiredKey);
            UserInterfaceHandler.instance.InsertButtonIntoVisualElement(variantButtonAreaId, variantButtonDefaultId, desiredKey, variantButtonTemplate);
            UserInterfaceHandler.instance.AddButtonRef(desiredKey);
            UserInterfaceHandler.instance.SetButtonLabel(desiredKey, variantIndex.ToString());

            // create new variant
            currentData.colorVariants.Add(new ColorVariant(desiredKey, currentData.originalColors));
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

        private void CreateVariantUI()
        {
            for(int i = 0; i < currentData.originalColors.Length; i++)
            {
                if(i == 0) { continue; }

                // Create ui from template
                int colorIndex = i;
                string desiredKey = colorEntryDefaultId + i;
                currentData.variantColorEntryIndexStrings.Add(colorIndex, desiredKey);

                TemplateContainer template = colorEntryTemplate.CloneTree();
                
                // get and give every element that needs to be accessed later an identifiable name
                VisualElement originalColorElement = template.Q<VisualElement>(colorEntryOriginalColorDefaultId);
                originalColorElement.name = colorEntryOriginalColorDefaultId + i;

                Label colorIndexLabel = template.Q<Label>(colorEntryIndexLabelDefaultId);
                colorIndexLabel.name = colorEntryIndexLabelDefaultId + i;

                Button newColorButton = template.Q<Button>(colorEntryNewColorButtonDefaultId);
                newColorButton.name = colorEntryNewColorButtonDefaultId + i;

                UserInterfaceHandler.instance.InsertElementIntoScrollView(colorEntryScrollViewId, colorEntryDefaultId, desiredKey, template);
                UserInterfaceHandler.instance.AddVisualElementRef(desiredKey);

                // handle ui
                UserInterfaceHandler.instance.AddVisualElementRef(colorEntryOriginalColorDefaultId + i);
                UserInterfaceHandler.instance.SetVisualElementBackgroundColor(colorEntryOriginalColorDefaultId + i, currentData.originalColors[i]);
                UserInterfaceHandler.instance.RemoveVisualElementRef(colorEntryOriginalColorDefaultId + i); // get rid of the reference if not needed anymore

                UserInterfaceHandler.instance.AddLabelRef(colorEntryIndexLabelDefaultId + i);
                UserInterfaceHandler.instance.SetLabel(colorEntryIndexLabelDefaultId + i, i.ToString());
                UserInterfaceHandler.instance.RemoveLabelRef(colorEntryIndexLabelDefaultId + i); // get rid of the reference if not needed anymore

                UserInterfaceHandler.instance.AddButtonRef(colorEntryNewColorButtonDefaultId + i);
                UserInterfaceHandler.instance.SetButtonBackgroundColor(colorEntryNewColorButtonDefaultId + i, currentData.originalColors[i]);
                UserInterfaceHandler.instance.AddButtonListener<int>(colorEntryNewColorButtonDefaultId + i, OnChangeColorButtonClicked, i);
                //Debug.Log($"Original color; index: {colorIndex}, Color: {currentData.originalColors[colorIndex]}");
            }
        }

        private void OnChangeColorButtonClicked(int index)
        {
            Debug.Log($"Clicked color index button: {index}");
        }

        private void UpdateUserInterface(DataHolder data)
        {
            UserInterfaceHandler.instance.AssignVisualElementBackground(originalSpriteId, currentData.originalTexture);

            Texture2D processedTexture = TextureUtils.GetShaderTexture(data.originalTexture, shaderMaterial);
            processedTexture.filterMode = FilterMode.Point;
            UserInterfaceHandler.instance.AssignVisualElementBackground(processedSpriteId, processedTexture);

            UserInterfaceHandler.instance.SetLabel(filenameLabelId , data.fileName);
        }

        private void ClearDynamicUserInterface()
        {
            if(currentData == null) { return; }

            // clean up dynamic ui listeners (and other garbage)
            for (int i = 0; i < currentData.originalColors.Length; i++)
            {
                UserInterfaceHandler.instance.RemoveButtonListener<int>(colorEntryNewColorButtonDefaultId + i.ToString());
                UserInterfaceHandler.instance.RemoveButtonRef(colorEntryNewColorButtonDefaultId + i.ToString());
                UserInterfaceHandler.instance.RemoveVisualElementRef(colorEntryDefaultId + i.ToString());
            }

            // remove all color variants
            for(int i = 0; i < currentData.colorVariants.Count; i++)
            {
                // remove any references
                UserInterfaceHandler.instance.RemoveButtonRef(variantButtonDefaultId + i.ToString());

            }

            UserInterfaceHandler.instance.ClearVisualElement(variantButtonAreaId);
            UserInterfaceHandler.instance.ClearScrollView(colorEntryScrollViewId);
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
