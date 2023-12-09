using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
	public PhotonView PV;
	public GameObject controller;

	public GameObject deathCam;

	UsernameDisplay username;

	int kills;
	int deaths;

	public bool isTagger = false;
	public bool isAlive = true;
	public Vector3 controllerPosition;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// Write your custom data to the stream
			stream.SendNext(isTagger);
		}
		else
		{
			// Read the custom data from the stream
			isTagger = (bool)stream.ReceiveNext();
		}
	}


	void Awake()
	{
		PV = GetComponent<PhotonView>();		
	}

	void Start()
	{
		if(PV.IsMine)
		{
			CreateController();
		}		
	}
    private void Update()
    {
		if (PV.IsMine)
			controllerPosition = controller.transform.position;
    }

    void CreateController()
	{
		Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
		controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bear"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
		username = controller.GetComponentInChildren<UsernameDisplay>();
	}

	void CreateDeathCam(Vector3 deathPosition)
	{
		deathPosition += new Vector3(0, 5f, 0);
		controller = Instantiate(deathCam, deathPosition, Quaternion.identity);	
	}

	public void Die()
	{
		PhotonNetwork.Destroy(controller);
		ToggleMouse.OnCursor();
		isTagger = false;
		isAlive = false;
		CreateDeathCam(controller.transform.position);
		//CreateController();
		//PhotonNetwork.LeaveRoom();


		// deaths++;

		// Hashtable hash = new Hashtable();
		// hash.Add("deaths", deaths);
		// PhotonNetwork.LocalPlayer.SetCustomProperties(hash);	
	}

	public void GetKill()
	{
		PV.RPC(nameof(RPC_GetKill), PV.Owner);
	}

	public void UpdateTaggers()
    {	
		int taggerIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["Tagger"];
		PV.RPC(nameof(RPC_NewTagger), RpcTarget.All, taggerIndex);	
	}

	public void SwapTagger(bool state)
	{
		//Debug.LogError(PV.Owner);
		isTagger = state;
		//PV.RPC(nameof(RPC_LocalTagger), RpcTarget.All, state);
	}


	#region RPC_FUNCTIONS
	[PunRPC]
	void RPC_GetKill()
	{
		kills++;

		Hashtable hash = new ();
		hash.Add("kills", kills);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
	}

	[PunRPC]
    void RPC_NewTagger(int taggerIndex)
    {
		//Debug.LogError("Owner:" + PhotonNetwork.LocalPlayer);
		// TAGGER
		TagManager.Instance.tagger = TagManager.Instance.existingPlayerList[taggerIndex];
		//Debug.LogError("The Tagger is " + TagManager.Instance.tagger);
		Find(TagManager.Instance.tagger).isTagger = true;
		//Debug.Log("Tagger Name:" + p.NickName + ". Player Name:" + PV.Owner.NickName + ".");

	}

	#endregion

	public static PlayerManager Find(Player player)
	{
		return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
	}


}