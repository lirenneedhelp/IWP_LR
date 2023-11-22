using UnityEngine;
using Photon.Pun;

public class UpdateItemQuantity : MonoBehaviour
{
    public int itemIndex;
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is a player
        PhotonView otherPhotonView = collision.gameObject.GetComponent<PhotonView>();
        if (otherPhotonView != null && otherPhotonView.IsMine)
        {
            // Call RPC on the stored GameObject
            UpdateInventory(itemIndex, otherPhotonView);
        }
        
    }
    public void UpdateInventory(int index, PhotonView pv)
    {
        ItemManager.Instance.pv.RPC(nameof(RPC_UpdateInventory), RpcTarget.All, index, pv.ViewID);
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    public void RPC_UpdateInventory(int index, int viewID)
    {
        PlayerManager playerManager = PlayerManager.Find(PhotonView.Find(viewID).Owner);
        playerManager.controller.GetComponent<PlayerController>().items[index].itemInfo.quantity++;
        return;
    }
}
