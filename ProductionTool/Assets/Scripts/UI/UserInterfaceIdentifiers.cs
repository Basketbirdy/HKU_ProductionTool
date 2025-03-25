using FileManagement;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "UserInterfaceIdentifiers", menuName = "ScriptableObjects/UserInterfaceIdentifiers")]
public class UserInterfaceIdentifiers : ScriptableObject
{
    [Header("Importing")]
    public string importButtonId = "Button_Import";

    [Header("Colors")]
    public string colorEntryScrollViewId = "ScrollView_ColorContainer";
    public string colorEntryDefaultId = "Element_VariantColorEntry";
    public string colorEntryOriginalColorDefaultId = "Element_OriginalColor";
    public string colorEntryIndexLabelDefaultId = "Label_VariantIndex";
    public string colorEntryNewColorButtonDefaultId = "Button_NewColor";
    [Header("ColorVariants")]
    public string addVariantButtonid = "Button_AddVariant";
    public string removeVariantButtonid = "Button_RemoveVariant";
    public string variantButtonAreaId = "VariantContainerMask";
    public string variantButtonDefaultId = "Button_Variant";
    public string selectedVariantLabelId = "Label_SelectedVariant";

    [Header("Saving")]
    public string saveButtonId = "Button_Save";
    public string saveAsButtonId = "Button_SaveAs";

    [Header("Exporting")]
    public string exportButtonId = "Button_Export";
    public string exportContentDropdownId = "Dropdown_ExportContent";
    public string exportFiletypeDropdownId = "Dropdown_ExportFiletype";

    [Header("Dynamic User Interface Ids")]
    public string originalSpriteId = "Sprite_Original";
    public string processedSpriteId = "Sprite_Processed";
    public string filenameLabelId = "Label_Filename";
}
