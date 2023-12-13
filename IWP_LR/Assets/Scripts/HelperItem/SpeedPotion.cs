using UnityEngine;
using Photon.Pun;

public class SpeedPotion : Potion
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
        TrailManager.Instance.SpawnTrail(potionEffectTrail.name, playerController.photonView.ViewID, 10f);
    }


}
