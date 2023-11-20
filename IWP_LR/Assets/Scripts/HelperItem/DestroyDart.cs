using System.Collections;
using Photon.Pun;
using UnityEngine;

public class DestroyDart : MonoBehaviourPun
{
    public float destroyDelay = 5f; // Adjust this value to set the delay before destruction

    void Start()
    {
        if (photonView.IsMine)
        {
            // Only the PhotonView owner should initiate the destruction
            StartCoroutine(DestroyAfterDelay());
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        // Destroy the Photon object
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
