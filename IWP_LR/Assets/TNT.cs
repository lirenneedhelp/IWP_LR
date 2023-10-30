using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TNT : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    [SerializeField] PhotonView playerPV;

    PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = playerPV.Owner == TagManager.Instance.tagger; 
        playerManager = PlayerManager.Find(playerPV.Owner);
    }
    void Update()
    {
        meshRenderer.enabled = playerManager.isTagger; 
    }


}
