using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UserInterfaceHandler : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private VisualElement root;

    Dictionary<string, Label> labels = new Dictionary<string, Label>();
    Dictionary<string, TextField> textFields = new Dictionary<string, TextField>();
    Dictionary<string, VisualElement> visualElements = new Dictionary<string, VisualElement>();

    Dictionary<string, Button> buttons = new Dictionary<string, Button>();
    Dictionary<string, Action> buttonLambdas = new Dictionary<string, Action>();

    Dictionary<string, DropdownField> dropdowns = new Dictionary<string, DropdownField>();
    Dictionary<string, ScrollView> scrollViews = new Dictionary<string, ScrollView>();

    public static UserInterfaceHandler instance;
    private void Awake()
    {
        if(instance == null) { instance = this; }
        else { Destroy(instance); }

        root = document.rootVisualElement;
    }

    public void AddLabelRef(string key)
    {
        if (labels.ContainsKey(key)) { return; }
        Label value = root.Q<Label>(key);
        if (value != null) { labels.Add(key, value); }
    }
    public void RemoveLabelRef(string key)
    {
        if (!labels.ContainsKey(key)) { return; }
        labels.Remove(key);
    }
    public void SetLabel(string key, string text)
    {
        if (!labels.ContainsKey(key)) { return; }
        labels[key].text = text;
    }

    public void AddTextFieldRef(string key)
    {
        if (textFields.ContainsKey(key)) { return; }
        TextField value = root.Q<TextField>(key);
        if (value != null) { textFields.Add(key, value); }
    }
    public void RemoveTextFieldRef(string key)
    {
        if (!textFields.ContainsKey(key)) { return; }
        textFields.Remove(key);
    }
    public string GetTextFieldText(string key)
    {
        if (!textFields.ContainsKey(key)) { return null; }
        return textFields[key].text;
    }
    public void SetTextFieldLabel(string key, string text)
    {
        if (!textFields.ContainsKey(key)) { return; }
        textFields[key].label = text;
    }

    public void AddVisualElementRef(string key)
    {
        if (visualElements.ContainsKey(key)) { return; }
        VisualElement value = root.Q<VisualElement>(key);
        if (value != null) { visualElements.Add(key, value); }
    }
    public void RemoveVisualElementRef(string key)
    {
        if (!visualElements.ContainsKey(key)) { return; }
        visualElements.Remove(key);
    }
    public void ShowVisualElement(string key)
    {
        if (!visualElements.ContainsKey(key)) { return; }
        visualElements[key].style.display = DisplayStyle.Flex;
    }
    public void HideVisualElement(string key)
    {
        if (!visualElements.ContainsKey(key)) { return; }
        visualElements[key].style.display = DisplayStyle.None;
    }
    public void AssignVisualElementBackground(string key, Texture2D texture)
    {
        if (!visualElements.ContainsKey(key)) { return; }
        visualElements[key].style.backgroundImage = texture;
    }
    public void AssignVisualElementBackground(string key, Sprite sprite)
    {
        if (!visualElements.ContainsKey(key)) { return; }
        visualElements[key].style.backgroundImage = new StyleBackground(sprite);
    }
    //public void InsertTemplateContainerIntoVisualElement(string key, VisualTreeAsset asset)
    //{
    //    if(!visualElements.ContainsKey(key)) { Debug.LogError("Visual element insertion target does not have a reference! throwing error"); return; }
    //    TemplateContainer template = asset.CloneTree();
    //    visualElements[key].Add(template);
    //    template.style.width = Length.Percent(25);
    //    template.style.height = Length.Percent(15);
    //}
    public void SetVisualElementBackgroundColor(string key, Color color)
    {
        if (!visualElements.ContainsKey(key)) { return; }
        visualElements[key].style.backgroundColor = new StyleColor(color);
    }
    public void InsertButtonIntoVisualElement(string key, string buttonAssetKey, string desiredKey, VisualTreeAsset asset)
    {
        if (!visualElements.ContainsKey(key)) { Debug.LogError("Visual element insertion target does not have a reference! throwing error"); return; }
        TemplateContainer template = asset.CloneTree();
        Button temp = template.Q<Button>(buttonAssetKey);
        temp.name = desiredKey;
        visualElements[key].Add(temp);
    }

    public void AddScrollViewRef(string key)
    {
        if (scrollViews.ContainsKey(key)) { return; }
        ScrollView value = root.Q<ScrollView>(key);
        if (value != null) { scrollViews.Add(key, value); }
    }
    public void RemoveScrollViewRef(string key)
    {
        if (!scrollViews.ContainsKey(key)) { return; }
        scrollViews.Remove(key);
    }
    public void InsertElementIntoScrollView(string key, string elementAssetKey, string desiredKey, TemplateContainer template)
    {
        VisualElement temp = template.Q<VisualElement>(elementAssetKey);
        temp.name = desiredKey;
        if (!scrollViews.ContainsKey(key)) { Debug.LogError("Visual element insertion target does not have a reference! throwing error"); return; }
        scrollViews[key].Add(temp);
    }

    public void AddButtonRef(string key)
    {
        if (buttons.ContainsKey(key)) { return; }
        Button value = root.Q<Button>(key);
        if (value != null) { buttons.Add(key, value); }
    }
    public void RemoveButtonRef(string key)
    {
        if (!buttons.ContainsKey(key)) { return; }
        buttons.Remove(key);
    }
    public void AddButtonListener(string key, System.Action action)
    {
        if (!buttons.ContainsKey(key)) { return; }
        buttons[key].clicked += action;
    }
    public void AddButtonListener<T>(string key, System.Action<T> action, T parameter)
    {
        if (!buttons.ContainsKey(key)) { return; }
        if (buttonLambdas.ContainsKey(key)) { buttonLambdas.Remove(key); }
        buttonLambdas.Add(key, () => action(parameter));
        buttons[key].clicked += buttonLambdas[key];
    }
    public void RemoveButtonListener(string key, System.Action action)
    {
        if (!buttons.ContainsKey(key)) { return; }
        buttons[key].clicked -= action;
    }
    public void RemoveButtonListener<T>(string key)
    {
        if (!buttons.ContainsKey(key)) { return; }
        if (!buttonLambdas.ContainsKey(key)) { return; }
        buttons[key].clicked -= buttonLambdas[key];
        buttonLambdas.Remove(key);
    }
    public void SetButtonLabel(string key, string msg)
    {
        if (!buttons.ContainsKey(key)) { return; }
        buttons[key].text = msg;
    }
    public void SetButtonBackgroundColor(string key, Color color)
    {
        if (!buttons.ContainsKey(key)) { return; }
        buttons[key].style.backgroundColor = new StyleColor(color);
    }

    public void AddDropdownRef(string key)
    {
        if(dropdowns.ContainsKey(key)) { return; }
        DropdownField value = root.Q<DropdownField>(key);
        if(value != null) { dropdowns.Add(key, value); }
    }
    public void RemoveDropdownRef(string key)
    {
        if (!dropdowns.ContainsKey(key)) { return; }
        dropdowns.Remove(key);
    }
    public int GetDropdownValue(string key)
    {
        if (!dropdowns.ContainsKey(key)) { return -1; }
        return dropdowns[key].index;
    }
    public void AddDropdownListener(string key, Action<ChangeEvent<string>> action)
    {
        if (!dropdowns.ContainsKey(key)) { return; }
        dropdowns[key].RegisterValueChangedCallback(evt => action.Invoke(evt));
    }
    public void RemoveDropdownListener(string key, Action<ChangeEvent<string>> action)
    {
        if (!dropdowns.ContainsKey(key)) { return; }
        dropdowns[key].UnregisterValueChangedCallback(evt => action.Invoke(evt));
    }

    public void ClearVisualElement(string key)
    {
        if(!visualElements.ContainsKey(key)) { return; }
        visualElements[key].Clear();
    }
    public void ClearScrollView(string key)
    {
        if (!scrollViews.ContainsKey(key)) { return; }
        scrollViews[key].Clear();
    }
}
