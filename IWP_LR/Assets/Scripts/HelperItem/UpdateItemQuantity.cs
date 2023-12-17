using UnityEngine;
using Photon.Pun;

public class UpdateItemQuantity : MonoBehaviourPun
{
    public int itemIndex;
    [SerializeField] PhotonView pv;
    private void OnTriggerEnter(Collider collider)
    {
        // Check if the collided object is a player
        PhotonView otherPhotonView = collider.gameObject.GetComponent<PhotonView>();
        if (otherPhotonView != null && otherPhotonView.IsMine)
        {
            // Call RPC on the stored GameObject
            ItemManager.Instance.UpdateInventory(itemIndex, otherPhotonView);
            ItemManager.Instance.pv.RPC("RPC_DestroyItem", RpcTarget.AllViaServer, pv.ViewID);
            //PhotonNetwork.Destroy(pv.gameObject);
        }
    }
   
}
