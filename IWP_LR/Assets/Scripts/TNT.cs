using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TNT : MonoBehaviour
{
    [SerializeField] MeshRenderer[] meshRenderer;

    [SerializeField] PhotonView playerPV;

    PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        //UpdateVisibility();
        playerManager = PlayerManager.Find(playerPV.Owner);
    }
    void Update()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        for (int i = 0; i < meshRenderer.Length; i++)
            meshRenderer[i].enabled = playerManager.isTagger;
    }
}
