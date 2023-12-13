using UnityEngine;
using Photon.Pun;

public class JumpPotion : Potion
{
    [SerializeField] float jump_boost;
    public PlayerController playerController;

    private void Start()
    {
    }

    public override void Use()
    {
        playerController.ApplyJumpBoost(jump_boost);
        TrailManager.Instance.SpawnTrail(potionEffectTrail.name, playerController.photonView.ViewID, 10f);
    }


}
