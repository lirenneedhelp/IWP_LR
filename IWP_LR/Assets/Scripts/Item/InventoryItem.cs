using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image image;
    //public Item Item;

    public void InitialiseItem(Item newItem)
    {
        image.sprite = newItem.itemInfo.displayImage;
        //Item = newItem;
    }
}
