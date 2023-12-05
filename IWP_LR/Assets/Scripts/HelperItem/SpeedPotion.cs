using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : HelperItem
{
    [SerializeField] float speed_boost;
    public PlayerController playerController;


    public override void Use()
    {
        Debug.Log("Using Speed Potion");
        IncreaseSpeed();
    }

    void IncreaseSpeed()
    {
        playerController.ApplySpeed(speed_boost);
    }
}
