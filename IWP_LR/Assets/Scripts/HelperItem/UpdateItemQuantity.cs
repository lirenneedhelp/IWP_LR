using UnityEngine;
using Photon.Pun;

public class UpdateItemQuantity : MonoBehaviourPun
{
    public int itemIndex;
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is a player
        PhotonView otherPhotonView = collision.gameObject.GetComponent<PhotonView>();
        if (otherPhotonView != null && otherPhotonView.IsMine)
        {
            // Call RPC on the stored GameObject
            ItemManager.Instance.UpdateInventory(itemIndex, otherPhotonView);
            ItemManager.Instance.pv.RPC("RPC_DestroyItem", RpcTarget.All, photonView.ViewID);
        }
    }
   
}
