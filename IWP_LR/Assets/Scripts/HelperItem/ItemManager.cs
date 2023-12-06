using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System.Linq;

public class ItemManager : MonoBehaviour, IPunObservable
{
    public static ItemManager Instance = null;
    public PhotonView pv;
    public ItemSpawnpoint[] itemSpawnpoints;
    public GameObject[] itemPrefabs;
    private int[] sceneItems;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            itemSpawnpoints = GetComponentsInChildren<ItemSpawnpoint>();
        }
    }
    private void Start()
    {
        sceneItems = Enumerable.Repeat(-1, itemSpawnpoints.Length).ToArray();

        if (PhotonNetwork.IsMasterClient)
            InvokeRepeating(nameof(SpawnItems), 0f, 20f);
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
            if (sceneItems[i] == -1)
            {
                int randomItemIndex = Random.Range(0, itemPrefabs.Length);
                //Debug.Log("Spawning Item");
                //Debug.Log(itemPrefabs[randomItemIndex].name);
                GameObject collectibleObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", itemPrefabs[randomItemIndex].name), itemSpawnpoints[i].transform.position, itemSpawnpoints[i].transform.rotation, 0, new object[] { pv.ViewID });

                //GameObject parentObject = new GameObject("ItemSpawnParent");
                //parentObject.transform.position = itemSpawnpoints[i].transform.position; 
                //collectibleObj.transform.parent = parentObject.transform;

                sceneItems[i] = collectibleObj.GetComponent<PhotonView>().ViewID;
                itemSpawnpoints[i].waypoint.SetActive(true);

            }
        }

    }

    [PunRPC]
    public void RPC_UpdateInventory(int index, int viewID)
    {
        PhotonView PV = PhotonView.Find(viewID);
        PlayerController pc = PV.gameObject.GetComponent<PlayerController>();

        if (pc != null)
        {
            var checkIfDart = itemPrefabs[index].GetComponent<SlownessDart>();
            if (checkIfDart != null)
                checkIfDart.cam = pc.cam;
            var checkIfJump = itemPrefabs[index].GetComponent<JumpPotion>();
            if (checkIfJump)
                checkIfJump.playerController = pc;
            var checkIfSpeed = itemPrefabs[index].GetComponent<SpeedPotion>();
            if (checkIfSpeed)
                checkIfSpeed.playerController = pc;

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
            for (int i = 0; i < sceneItems.Length; i++)
            {
                if (itemPV.ViewID == sceneItems[i])
                {
                    sceneItems[i] = -1;
                    itemSpawnpoints[i].waypoint.SetActive(false);
                    break;
                }
            }

            PhotonNetwork.Destroy(itemPV.gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Write your custom data to the stream
            stream.SendNext(sceneItems);
        }
        else
        {
            // Read the custom data from the stream
            sceneItems = (int[])stream.ReceiveNext();
        }
    }
}
