using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image slotImage;

    public Color selectedColor, DeselectColor;

    public InventoryItem item;

    public void Selected()
    {
        slotImage.color = selectedColor;
    }

    public void Deselect()
    {
        slotImage.color = DeselectColor;
    }
}
