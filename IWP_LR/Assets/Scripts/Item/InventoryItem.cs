using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public Image image;
    public TMP_Text countText;

    public int count = 1;
    public Item Item;


    public void InitialiseItem(Item newItem)
    {
        image.sprite = newItem.itemInfo.displayImage;
        Item = newItem;
        RefreshCount();
    }

    public void RefreshCount()
    {
        if (count < 1)
        {
            Destroy(gameObject);
            return;
        }

        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
}
