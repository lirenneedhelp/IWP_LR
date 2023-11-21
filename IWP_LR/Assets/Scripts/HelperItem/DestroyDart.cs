using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DestroyDart : MonoBehaviourPun
{
    void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
            return;

        // Check if the collided object is a player
        PhotonView otherPhotonView = collision.gameObject.GetComponent<PhotonView>();
        if (otherPhotonView != null && otherPhotonView.IsMine)
        {
            // Store the GameObject and PhotonViewID before destroying the dart
            int targetViewID = otherPhotonView.ViewID;
            GameObject targetObject = otherPhotonView.gameObject;

            // Call RPC on the stored GameObject
            photonView.RPC(nameof(RPC_ShootDart), RpcTarget.All, targetViewID);
        }     

    }

    [PunRPC]
    void RPC_ShootDart(int viewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(viewID);

        if (targetPhotonView != null)
        {
            // Check if the target is the local player
            if (targetPhotonView.IsMine)
            {
                // Handle the impact on the target (e.g., remove a part, trigger an animation)
                Debug.Log("Dart hit the target!");

                // Get the PlayerController component from the target
                PlayerController pc = targetPhotonView.gameObject.GetComponent<PlayerController>();

                // Apply debuff using the PlayerController
                if (pc != null)
                {
                    Debug.Log("Applied Debuff");
                    pc.ApplyDebuff();
                }
            }
        }

        // Destroy the dart on the shooter's client after RPC is completed
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(photonView.gameObject);
        }

    }

}
