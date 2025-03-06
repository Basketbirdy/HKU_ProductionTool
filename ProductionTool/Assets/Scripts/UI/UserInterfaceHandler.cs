using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UserInterfaceHandler : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private VisualElement root;

    Dictionary<string, Label> labels = new Dictionary<string, Label>();
    Dictionary<string, VisualElement> visualElements = new Dictionary<string, VisualElement>();
    Dictionary<string, Button> buttons = new Dictionary<string, Button>();

    public static UserInterfaceHandler instance;
    private void Awake()
    {
        if(instance == null) { instance = this; }
        else { Destroy(instance); }
    }

    public void AddLabelRef(string key)
    {
        if (labels.ContainsKey(key)) { return; }
        Label value = root.Q<Label>(key);
        if (value != null) { labels.Add(key, value); }
    }
    public void ChangeLabel(string key, string text)
    {
        if (!labels.ContainsKey(key)) { return; }
        labels[key].text = text;
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
