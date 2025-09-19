using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles the Saturation/Value (SV) picker interactions inside the color picker UI
public class SVImageControll : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] private Image pickerImage;         
    [SerializeField] private ColourPicker CC;           
    [SerializeField] private RectTransform rectTransform; 
    [SerializeField] private RectTransform pickerTransform; 

    // Updates SV values and picker position based on pointer event
    void UpdateColor(PointerEventData eventData)
    {
        // Convert screen position into local coordinates of the SV image
        Vector3 pos = rectTransform.InverseTransformPoint(eventData.position);

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        // Clamp so picker stays inside the SV area
        pos.x = Mathf.Clamp(pos.x, -deltaX, deltaX);
        pos.y = Mathf.Clamp(pos.y, -deltaY, deltaY);

        // Convert to normalized 0-1 range
        float x = pos.x + deltaX;
        float y = pos.y + deltaY;

        float xNorm = x / rectTransform.sizeDelta.x; // saturation
        float yNorm = y / rectTransform.sizeDelta.y; // value

        // Move picker marker
        pickerTransform.localPosition = pos;

        // Update picker marker color 
        pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

        // Tell ColourPicker about new values
        CC.SetSV(xNorm, yNorm);
    }

    // Places picker at the correct position given saturation & value
    public void SetPickerPosition(float saturation, float value)
    {
        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        // Map saturation/value back into pixel coordinates
        float x = saturation * rectTransform.sizeDelta.x;
        float y = value * rectTransform.sizeDelta.y;

        Vector3 pos = Vector3.zero;
        pos.x = x - deltaX;
        pos.y = y - deltaY;

        pickerTransform.localPosition = pos;
    }

    // Called when dragging inside SV area
    public void OnDrag(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }

    // Called when clicking inside SV area
    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColor(eventData);
    }
}
