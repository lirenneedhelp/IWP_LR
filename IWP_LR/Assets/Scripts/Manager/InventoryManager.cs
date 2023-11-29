using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] int maxStacked = 10;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    // Start is called before the first frame update
   public void AddItem(Item item)
   {
        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null &&
                itemInSlot.Item == item &&
                itemInSlot.count < maxStacked &&
                itemInSlot.Item.itemInfo.stackable == true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return;
            }

        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot, i);
                return;
            }
            
        }
   }
    
   void SpawnNewItem(Item item, InventorySlot slot, int slot_index)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventorySlots[slot_index].item = inventoryItem;
    }
}
