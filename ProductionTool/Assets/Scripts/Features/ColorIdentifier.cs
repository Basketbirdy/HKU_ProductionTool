using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ColorIdentifier : MonoBehaviour
{
    [SerializeField] UserInterfaceIdentifiers ids;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UserInterfaceHandler.instance.AddVisualElementRef(ids.processedSpriteId);
        UserInterfaceHandler.instance.RegisterPointerDownCallbackVisualElement(ids.processedSpriteId, OnPointerDown);
    }

    private void OnDisable()
    {
        UserInterfaceHandler.instance.UnregisterPointerDownCallbackVisualElement(ids.processedSpriteId, OnPointerDown);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        Vector2Int screenPos = new Vector2Int(Mathf.RoundToInt(Input.mousePosition.x), Mathf.RoundToInt(Input.mousePosition.y));
        Debug.Log($"Screen position: {screenPos}");
    }
}
