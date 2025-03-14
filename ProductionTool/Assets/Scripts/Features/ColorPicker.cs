using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using NUnit.Framework.Constraints;
using System.Linq;

[System.Serializable]
public class ColorPicker
{
    [SerializeField] private Color32 selectedColor;

    [SerializeField] private string colorPickerId = "Element_ColorPickerBackground";

    [SerializeField] private string oldColorIndicatorId = "Element_ColorPicker_OldColor";
    [SerializeField] private string newColorIndicatorId = "Element_ColorPicker_NewColor";

    [SerializeField] private string redSliderId = "SliderInt_ColorPicker_Red";
    [SerializeField] private string greenSliderId = "SliderInt_ColorPicker_Green";
    [SerializeField] private string blueSliderId = "SliderInt_ColorPicker_Blue";
    [SerializeField] private string redIntegerFieldId = "IntegerField_ColorPicker_Red";
    [SerializeField] private string greenIntegerFieldId = "IntegerField_ColorPicker_Green";
    [SerializeField] private string blueIntegerFieldId = "IntegerField_ColorPicker_Blue";
    [SerializeField] private string hexTextFieldId = "TextField_ColorPicker_Hex";

    [SerializeField] private string applyButtonId = "Button_ColorPicker_Apply";
    [SerializeField] private string cancelButtonId = "Button_ColorPicker_Cancel";

    private int redValue;
    private int greenValue;
    private int blueValue;
    private int alphaValue = 255;

    private char[] hexChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F' };
    Dictionary<char, int> charValues = new Dictionary<char, int>();
    Dictionary<int, char> valueChars = new Dictionary<int, char>();


    // state
    private bool isOpen;
    private int colorIndex;
    private System.Action<Color32, int> onPickerClosed;

    public void Initialize()
    {
        hexChars = new[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f',
        };
        for (int i = 0; i < hexChars.Length; i++)
        {
            charValues.Add(hexChars[i], i);
            valueChars.Add(i, hexChars[i]);
        }

        UserInterfaceHandler.instance.AddVisualElementRef(colorPickerId);

        UserInterfaceHandler.instance.AddVisualElementRef(oldColorIndicatorId);
        UserInterfaceHandler.instance.AddVisualElementRef(newColorIndicatorId);

        UserInterfaceHandler.instance.AddSliderIntRef(redSliderId);
        UserInterfaceHandler.instance.AddSliderIntRef(greenSliderId);
        UserInterfaceHandler.instance.AddSliderIntRef(blueSliderId);

        UserInterfaceHandler.instance.AddIntegerFieldRef(redIntegerFieldId);
        UserInterfaceHandler.instance.AddIntegerFieldRef(greenIntegerFieldId);
        UserInterfaceHandler.instance.AddIntegerFieldRef(blueIntegerFieldId);

        //UserInterfaceHandler.instance.AddTextFieldRef(hexTextFieldId);

        UserInterfaceHandler.instance.AddSliderIntListener(redSliderId, OnSliderValueChanged);
        UserInterfaceHandler.instance.AddSliderIntListener(greenSliderId, OnSliderValueChanged);
        UserInterfaceHandler.instance.AddSliderIntListener(blueSliderId, OnSliderValueChanged);

        UserInterfaceHandler.instance.AddIntegerFieldListener(redIntegerFieldId, OnIntegerFieldValueChanged);
        UserInterfaceHandler.instance.AddIntegerFieldListener(greenIntegerFieldId, OnIntegerFieldValueChanged);
        UserInterfaceHandler.instance.AddIntegerFieldListener(blueIntegerFieldId, OnIntegerFieldValueChanged);

        UserInterfaceHandler.instance.AddButtonRef(applyButtonId);
        UserInterfaceHandler.instance.AddButtonRef(cancelButtonId);

        //UserInterfaceHandler.instance.AddTextFieldListener(hexTextFieldId, OnTextFieldValueChanged);

        UserInterfaceHandler.instance.HideVisualElement(colorPickerId);
    }

    public void Open(Color32 openingColor, int index)
    {
        isOpen = true;
        colorIndex = index;
        UserInterfaceHandler.instance.ShowVisualElement(colorPickerId);

        // set all values to openingColor
        UserInterfaceHandler.instance.SetVisualElementBackgroundColor(oldColorIndicatorId, openingColor);
        UserInterfaceHandler.instance.SetVisualElementBackgroundColor(newColorIndicatorId, openingColor);

        redValue = openingColor.r;
        greenValue = openingColor.g;
        blueValue = openingColor.b;
        alphaValue = 255;

        SetColorSliders();
        SetColorIntegerFields();

        UserInterfaceHandler.instance.AddButtonListener(applyButtonId, OnApplyButtonClicked);
        UserInterfaceHandler.instance.AddButtonListener(cancelButtonId, OnCancelButtonClicked);

        selectedColor = new Color32((byte)redValue, (byte)greenValue, (byte)blueValue, (byte)alphaValue);
        Debug.Log($"New selected color is {selectedColor}");
    }

    public void OnApplyButtonClicked()
    {
        Debug.Log($"Confirming color picked");

        UserInterfaceHandler.instance.RemoveButtonListener(applyButtonId, OnApplyButtonClicked);
        UserInterfaceHandler.instance.RemoveButtonListener(cancelButtonId, OnCancelButtonClicked);

        Close();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log($"Cancelling color picked");

        UserInterfaceHandler.instance.RemoveButtonListener(applyButtonId, OnApplyButtonClicked);
        UserInterfaceHandler.instance.RemoveButtonListener(cancelButtonId, OnCancelButtonClicked);

        selectedColor = new Color(0, 0, 0, 0);
        Close();
    }

    private void Close()
    {
        UserInterfaceHandler.instance.HideVisualElement(colorPickerId);
        isOpen = false;
        onPickerClosed?.Invoke(selectedColor, colorIndex);
    }

    private void OnTextFieldValueChanged(ChangeEvent<string> evt)
    {
        string newValue = evt.newValue;

        bool isValid = true;
        foreach (char c in newValue)
        {
            if (hexChars.Contains(c))
            {
                isValid = false;
                break;
            }

            if (!charValues.ContainsKey(c))
            {
                isValid = false;
                break;
            }
        }
        if (newValue.Length > 6) { isValid = false; }

        if (newValue.Length < 6)
        {
            int unfilled = 6 - newValue.Length;
            for (int i = 6; i > newValue.Length; i--)
            {
                newValue += 0.ToString();
            }
        }

        if (!isValid)
        {
            UserInterfaceHandler.instance.SetTextFieldValueWithoutNotify(hexTextFieldId, evt.previousValue);
            return;
        }

        selectedColor = HexToColor(newValue);

        RecalculateValues();
    }

    private Color HexToColor(string hex)
    {
        int r1 = charValues[hex[0]];
        int r2 = charValues[hex[1]];
        int r = (r1 * 16) + r2;

        int g1 = charValues[hex[2]];
        int g2 = charValues[hex[3]];
        int g = (g1 * 16) + g2;

        int b1 = charValues[hex[4]];
        int b2 = charValues[hex[5]];
        int b = (b1 * 16) + b2;

        return new Color(r, g, b, 255);
    }
    private string ColorToHex(Color32 color)
    {
        int r1 = Mathf.FloorToInt(color.r / 16);
        int r2 = color.r % 16;
        char red1 = valueChars[r1];
        char red2 = valueChars[r2];

        int g1 = Mathf.FloorToInt(color.g / 16);
        int g2 = color.g % 16;
        char green1 = valueChars[g1];
        char green2 = valueChars[g2];

        int b1 = Mathf.FloorToInt(color.b / 16);
        int b2 = color.g % 16;
        char blue1 = valueChars[b1];
        char blue2 = valueChars[b2];

        return string.Join("", red1, red2, green1, green2, blue1, blue2);
    }

    private void OnSliderValueChanged(ChangeEvent<int> evt)
    {
        Debug.Log($"SliderInt changed to: {evt.newValue}");

        redValue = UserInterfaceHandler.instance.GetSliderIntValue(redSliderId);
        greenValue = UserInterfaceHandler.instance.GetSliderIntValue(greenSliderId);
        blueValue = UserInterfaceHandler.instance.GetSliderIntValue(blueSliderId);

        SetColorSliders();
        SetColorIntegerFields();

        selectedColor = new Color32((byte)redValue, (byte)greenValue, (byte)blueValue, (byte)alphaValue);
        Debug.Log($"New selected color is {selectedColor}");

        SetNewColorIndicator(selectedColor);
    }

    private void OnIntegerFieldValueChanged(ChangeEvent<int> evt)
    {
        Debug.Log($"Integer field changed to: {evt.newValue}");

        redValue = UserInterfaceHandler.instance.GetIntegerFieldValue(redIntegerFieldId);
        greenValue = UserInterfaceHandler.instance.GetIntegerFieldValue(greenIntegerFieldId);
        blueValue = UserInterfaceHandler.instance.GetIntegerFieldValue(blueIntegerFieldId);

        SetColorSliders();
        SetColorIntegerFields();

        selectedColor = new Color32((byte)redValue, (byte)greenValue, (byte)blueValue, (byte)alphaValue);
        Debug.Log($"New selected color is {selectedColor}");

        SetNewColorIndicator(selectedColor);
    }

    private void RecalculateValues()
    {
        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(redSliderId, selectedColor.r);
        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(redIntegerFieldId, selectedColor.r);

        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(greenSliderId, selectedColor.g);
        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(greenIntegerFieldId, selectedColor.g);

        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(blueSliderId, selectedColor.b);
        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(blueIntegerFieldId, selectedColor.b);

        UserInterfaceHandler.instance.SetTextFieldValueWithoutNotify(hexTextFieldId, ColorToHex(selectedColor));
    }

    private void SetColorSliders()
    {
        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(redSliderId, redValue);
        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(greenSliderId, greenValue);
        UserInterfaceHandler.instance.SetSliderIntValueWithoutNotify(blueSliderId, blueValue);
    }

    private void SetColorIntegerFields()
    {
        UserInterfaceHandler.instance.SetIntegerFieldValueWithoutNotify(redIntegerFieldId, redValue);
        UserInterfaceHandler.instance.SetIntegerFieldValueWithoutNotify(greenIntegerFieldId, greenValue);
        UserInterfaceHandler.instance.SetIntegerFieldValueWithoutNotify(blueIntegerFieldId, blueValue);
    }

    private void SetNewColorIndicator(Color color)
    {
        Debug.Log($"specified new indicator color is {color}");
        UserInterfaceHandler.instance.SetVisualElementBackgroundColor(newColorIndicatorId, color);
    }

    public void AddColorListener(Action<Color32, int> action)
    {
        onPickerClosed += action;
    }
    public void RemoveColorListener(Action<Color32, int> action)
    {
        onPickerClosed -= action;
    }
    public bool GetState()
    {
        return isOpen;
    }
}
