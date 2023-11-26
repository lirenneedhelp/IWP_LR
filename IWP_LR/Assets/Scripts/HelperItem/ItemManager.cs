using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance = null;
    public PhotonView pv;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateInventory(int index, PhotonView pv)
    {
        this.pv.RPC(nameof(RPC_UpdateInventory), RpcTarget.All, index, pv.ViewID);
    }
    [PunRPC]
    public void RPC_UpdateInventory(int index, int viewID)
    {
        PlayerManager playerManager = PlayerManager.Find(PhotonView.Find(viewID).Owner);
        playerManager.controller.GetComponent<PlayerController>().items[index].itemInfo.quantity++;
        //Destroy(gameObject);
        return;
    }

}
