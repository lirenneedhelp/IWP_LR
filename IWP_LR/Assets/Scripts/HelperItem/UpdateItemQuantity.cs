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
            ItemManager.Instance.UpdateInventory(itemIndex, otherPhotonView);
        }
    }
   
}
