using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class UserInterfaceHandler : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private VisualElement root;

    Dictionary<string, Label> labels = new Dictionary<string, Label>();
    Dictionary<string, TextField> textFields = new Dictionary<string, TextField>();
    Dictionary<string, VisualElement> visualElements = new Dictionary<string, VisualElement>();
    Dictionary<string, Button> buttons = new Dictionary<string, Button>();

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

    public void AddButtonRef(string key)
    {
        if (buttons.ContainsKey(key)) { return; }
        Button value = root.Q<Button>(key);
        if (value != null) { buttons.Add(key, value); }
    }
    public void AddButtonListener(string key, System.Action action)
    {
        if (!buttons.ContainsKey(key)) { return; }
        buttons[key].clicked += action;
    }
    public void RemoveButtonListener(string key, System.Action action)
    {
        if (!buttons.ContainsKey(key)) { return; }
        buttons[key].clicked -= action;
    }
}
