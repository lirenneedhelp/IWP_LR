using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrailManager : MonoBehaviourPun
{
    public static TrailManager Instance = null;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public void SpawnTrail(string trailName, int playerID, float trailDuration)
    {
        photonView.RPC(nameof(RPC_SpawnTrail), RpcTarget.All, trailName, playerID, trailDuration);
    }

    [PunRPC]
    public void RPC_SpawnTrail(string trailName, int playerID, float trailDuration)
    {
        Transform playerTransform = PhotonView.Find(playerID).transform;
        GameObject trailObject = PhotonNetwork.Instantiate(trailName, playerTransform.position, playerTransform.rotation);

        trailObject.transform.parent = playerTransform;
        StartCoroutine(DestroyingParticle(trailDuration, trailObject));
    }

    private IEnumerator DestroyingParticle(float duration, GameObject GO)
    {
        yield return new WaitForSeconds(duration);

        PhotonNetwork.Destroy(GO);
    }
}


