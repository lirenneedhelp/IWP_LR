using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : ScriptableObject
{
	public string itemName;
	public int quantity;
	public Sprite displayImage;

	public bool stackable;
}