using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMouse : MonoBehaviour
{
    public static void OnCursor()
    {
        Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
    }

    public static void OffCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }
}
