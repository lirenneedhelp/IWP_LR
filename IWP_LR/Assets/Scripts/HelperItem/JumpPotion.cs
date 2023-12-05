using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class JumpPotion : HelperItem
{
    [SerializeField] float jump_boost;
    public PlayerController playerController;

    private void Start()
    {
    }

    public override void Use()
    {
        playerController.ApplyJumpBoost(jump_boost);
    }
}
