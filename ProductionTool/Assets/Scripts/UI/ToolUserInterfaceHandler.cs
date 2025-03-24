using System;
using UnityEngine;
using UnityEngine.UIElements;
using FileManagement;

public class ToolUserInterfaceHandler
{
    private UserInterfaceIdentifiers ids; // string id's for all the ui toolkit ui elements

    // ui toolkit documents
    private VisualTreeAsset variantButtonTemplate;
    private VisualTreeAsset colorEntryTemplate;

    public ToolUserInterfaceHandler(UserInterfaceIdentifiers ids, VisualTreeAsset variantButtonTemplate, VisualTreeAsset colorEntryTemplate)
    {
        this.ids = ids;

        this.variantButtonTemplate = variantButtonTemplate;
        this.colorEntryTemplate = colorEntryTemplate;
    }

    // initialisation ---------------------------------------------------------------
    public void InitializeReferences()
    {
        // static ui
            // buttons
        UserInterfaceHandler.instance.AddButtonRef(ids.importButtonId);

        UserInterfaceHandler.instance.AddButtonRef(ids.exportButtonId);

        UserInterfaceHandler.instance.AddButtonRef(ids.saveButtonId);
        UserInterfaceHandler.instance.AddButtonRef(ids.saveAsButtonId);

        UserInterfaceHandler.instance.AddButtonRef(ids.addVariantButtonid);
        UserInterfaceHandler.instance.AddButtonRef(ids.removeVariantButtonid);
            // dropdowns
        UserInterfaceHandler.instance.AddDropdownRef(ids.exportContentDropdownId);
        UserInterfaceHandler.instance.AddDropdownRef(ids.exportFiletypeDropdownId);

        // dynamic ui
            // visual elements - images
        UserInterfaceHandler.instance.AddVisualElementRef(ids.originalSpriteId);
        UserInterfaceHandler.instance.AddVisualElementRef(ids.processedSpriteId);

        UserInterfaceHandler.instance.AddVisualElementRef(ids.variantButtonAreaId);
            // labels - text
        UserInterfaceHandler.instance.AddLabelRef(ids.filenameLabelId);

            // scrollviews
        UserInterfaceHandler.instance.AddScrollViewRef(ids.colorEntryScrollViewId);
    }
    public void InitializeButtons(Action importAction, Action exportAction, Action saveAction, Action saveAsAction, Action addVariantAction, 
                                    Action removeVariantAction, Action<ChangeEvent<string>> contentChangeAction, Action<ChangeEvent<string>> fileTypeChangeAction )
    {
        UserInterfaceHandler.instance.AddButtonListener(ids.importButtonId, importAction);
        UserInterfaceHandler.instance.AddButtonListener(ids.exportButtonId, exportAction);
        UserInterfaceHandler.instance.AddButtonListener(ids.saveButtonId, saveAction);
        UserInterfaceHandler.instance.AddButtonListener(ids.saveAsButtonId, saveAsAction);
        UserInterfaceHandler.instance.AddButtonListener(ids.addVariantButtonid, addVariantAction);
        UserInterfaceHandler.instance.AddButtonListener(ids.removeVariantButtonid, removeVariantAction);
        UserInterfaceHandler.instance.AddDropdownListener(ids.exportContentDropdownId, contentChangeAction);
        UserInterfaceHandler.instance.AddDropdownListener(ids.exportFiletypeDropdownId, fileTypeChangeAction);
    }

    public void OnDisable(int colorsLength, Action importAction, Action exportAction, Action saveAction, Action saveAsAction, Action addVariantAction,
                            Action removeVariantAction, Action<ChangeEvent<string>> contentChangeAction, Action<ChangeEvent<string>> fileTypeChangeAction)
    {
        // static ui
        // buttons
        UserInterfaceHandler.instance.RemoveButtonListener(ids.importButtonId, importAction);
        UserInterfaceHandler.instance.RemoveButtonListener(ids.exportButtonId, exportAction);

        UserInterfaceHandler.instance.RemoveButtonListener(ids.saveButtonId, saveAction);
        UserInterfaceHandler.instance.RemoveButtonListener(ids.saveAsButtonId, saveAsAction);

        UserInterfaceHandler.instance.RemoveButtonListener(ids.addVariantButtonid, addVariantAction);
        UserInterfaceHandler.instance.RemoveButtonListener(ids.removeVariantButtonid, removeVariantAction);

        // dropdowns
        UserInterfaceHandler.instance.RemoveDropdownListener(ids.exportContentDropdownId, contentChangeAction);
        UserInterfaceHandler.instance.RemoveDropdownListener(ids.exportFiletypeDropdownId, fileTypeChangeAction);

        // dynamic ui
        for (int i = 0; i < colorsLength; i++)
        {
            UserInterfaceHandler.instance.RemoveButtonListener<int>(ids.colorEntryNewColorButtonDefaultId + i);
        }
    }

    // updating user interface elements ---------------------------------------------
    public void UpdateColorEntryButtons(int originalColorLength, Color[] newColors)
    {
        for (int i = 0; i < originalColorLength; i++)
        {
            UserInterfaceHandler.instance.SetButtonBackgroundColor(ids.colorEntryNewColorButtonDefaultId + i, newColors[i]);
        }
    }

    // shader processed image
    public void UpdateProcessedImage(DataHolder currentData, Material shaderMaterial)
    {
        UpdateShaderInfo(currentData, shaderMaterial);
        UpdateShaderImage(currentData, shaderMaterial);
    }
    private void UpdateShaderInfo(DataHolder currentData, Material shaderMaterial)
    {
        Texture2D baseTexture = TextureUtils.CreateColorTexture1D(currentData.originalColors);
        baseTexture.filterMode = FilterMode.Point;
        shaderMaterial.SetTexture("_BaseColors", baseTexture);

        Texture2D outputTexture = TextureUtils.CreateColorTexture1D(currentData.colorVariants[currentData.selectedIndex].newColors);
        outputTexture.filterMode = FilterMode.Point;
        shaderMaterial.SetTexture("_OutputColors", outputTexture);
    }
    private void UpdateShaderImage(DataHolder currentData, Material shaderMaterial)
    {
        Texture2D processedTexture = TextureUtils.GetShaderTexture(currentData.originalTexture, shaderMaterial);
        processedTexture.filterMode = FilterMode.Point;
        UserInterfaceHandler.instance.AssignVisualElementBackground(ids.processedSpriteId, processedTexture);
    }

    // dynamic user interface
    public void CreateVariantButton(int variantIndex, string desiredKey, Action<int> buttonAction)
    {
        UserInterfaceHandler.instance.InsertButtonIntoVisualElement(ids.variantButtonAreaId, ids.variantButtonDefaultId, desiredKey, variantButtonTemplate);
        UserInterfaceHandler.instance.AddButtonRef(desiredKey);
        UserInterfaceHandler.instance.SetButtonLabel(desiredKey, variantIndex.ToString());
        UserInterfaceHandler.instance.AddButtonListener<int>(desiredKey, buttonAction, variantIndex);
    }
    public void CreateColorEntry(int entryIndex, string desiredKey, Color originalColor, Action<int> buttonAction)
    {
        TemplateContainer template = colorEntryTemplate.CloneTree();

        // get and give every element that needs to be accessed later an identifiable name
        VisualElement originalColorElement = template.Q<VisualElement>(ids.colorEntryOriginalColorDefaultId);
        originalColorElement.name = ids.colorEntryOriginalColorDefaultId + entryIndex;

        Label colorIndexLabel = template.Q<Label>(ids.colorEntryIndexLabelDefaultId);
        colorIndexLabel.name = ids.colorEntryIndexLabelDefaultId + entryIndex;

        Button newColorButton = template.Q<Button>(ids.colorEntryNewColorButtonDefaultId);
        newColorButton.name = ids.colorEntryNewColorButtonDefaultId + entryIndex;

        UserInterfaceHandler.instance.InsertElementIntoScrollView(ids.colorEntryScrollViewId, ids.colorEntryDefaultId, desiredKey, template);
        UserInterfaceHandler.instance.AddVisualElementRef(desiredKey);

        // handle ui
        UserInterfaceHandler.instance.AddVisualElementRef(ids.colorEntryOriginalColorDefaultId + entryIndex);
        UserInterfaceHandler.instance.SetVisualElementBackgroundColor(ids.colorEntryOriginalColorDefaultId + entryIndex, originalColor);
        UserInterfaceHandler.instance.RemoveVisualElementRef(ids.colorEntryOriginalColorDefaultId + entryIndex); // get rid of the reference if not needed anymore

        UserInterfaceHandler.instance.AddLabelRef(ids.colorEntryIndexLabelDefaultId + entryIndex);
        UserInterfaceHandler.instance.SetLabel(ids.colorEntryIndexLabelDefaultId + entryIndex, entryIndex.ToString());
        UserInterfaceHandler.instance.RemoveLabelRef(ids.colorEntryIndexLabelDefaultId + entryIndex); // get rid of the reference if not needed anymore

        UserInterfaceHandler.instance.AddButtonRef(ids.colorEntryNewColorButtonDefaultId + entryIndex);
        UserInterfaceHandler.instance.SetButtonBackgroundColor(ids.colorEntryNewColorButtonDefaultId + entryIndex, originalColor);
        UserInterfaceHandler.instance.AddButtonListener<int>(ids.colorEntryNewColorButtonDefaultId + entryIndex, buttonAction, entryIndex);
        //Debug.Log($"Original color; index: {colorIndex}, Color: {currentData.originalColors[colorIndex]}");
    }
    public void ClearDynamicUserInterface(int originalColorLength, int variantCount)
    {
        // clean up dynamic ui listeners (and other garbage)
        for (int i = 0; i < originalColorLength; i++)
        {
            UserInterfaceHandler.instance.RemoveButtonListener<int>(ids.colorEntryNewColorButtonDefaultId + i.ToString());
            UserInterfaceHandler.instance.RemoveButtonRef(ids.colorEntryNewColorButtonDefaultId + i.ToString());
            UserInterfaceHandler.instance.RemoveVisualElementRef(ids.colorEntryDefaultId + i.ToString());
        }

        // remove all color variants
        for (int i = 0; i < variantCount; i++)
        {
            // remove any references
            UserInterfaceHandler.instance.RemoveButtonListener<int>(ids.variantButtonDefaultId + i.ToString());
            UserInterfaceHandler.instance.RemoveButtonRef(ids.variantButtonDefaultId + i.ToString());

        }

        UserInterfaceHandler.instance.ClearVisualElement(ids.variantButtonAreaId);
        UserInterfaceHandler.instance.ClearScrollView(ids.colorEntryScrollViewId);
    }

}
