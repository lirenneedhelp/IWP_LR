using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance = null;
    public PhotonView pv;
    public Spawnpoint[] itemSpawnpoints;
    public GameObject[] itemPrefabs;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
       // itemSpawnpoints = GetComponentsInChildren<Spawnpoint>();
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            SpawnItems();
    }

    public void UpdateInventory(int index, PhotonView pv)
    {
        this.pv.RPC(nameof(RPC_UpdateInventory), pv.Owner, index, pv.ViewID);
    }
    public void SpawnItems()
    {
        Random.InitState(2);
        Debug.Log(itemSpawnpoints.Length);
        for (int i = 0; i < itemSpawnpoints.Length; i++)
        {
            int randomItemIndex = Random.Range(0, itemPrefabs.Length);
            //Debug.Log("Spawning Item");
            //Debug.Log(itemPrefabs[randomItemIndex].name);
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", itemPrefabs[randomItemIndex].name), itemSpawnpoints[i].transform.position, itemSpawnpoints[i].transform.rotation, 0, new object[] { pv.ViewID });
        }

    }

    [PunRPC]
    public void RPC_UpdateInventory(int index, int viewID)
    {
        PhotonView PV = PhotonView.Find(viewID);
        PlayerController pc = PV.gameObject.GetComponent<PlayerController>();

        if (pc != null)
        {
            itemPrefabs[index].GetComponent<SlownessDart>().cam = pc.cam;
            pc.inventoryManager.AddItem(itemPrefabs[index].GetComponent<ItemType>().item);
        }
    }

    [PunRPC]
    public void RPC_DestroyItem(int itemViewID)
    {
        PhotonView itemPV = PhotonView.Find(itemViewID);

        if (itemPV != null)
        {
            Debug.Log($"Before Ownership Transfer - IsMine: {itemPV.IsMine}, Owner: {itemPV.Owner}");

            // Transfer ownership to the master client
            //if (PhotonNetwork.IsMasterClient)
            {
                itemPV.TransferOwnership(PhotonNetwork.MasterClient);
            }

            Debug.Log($"After Ownership Transfer - IsMine: {itemPV.IsMine}, Owner: {itemPV.Owner}");

            // Destroy the object (now the master client should have ownership)
            PhotonNetwork.Destroy(itemPV.gameObject);
        }
    }


}
