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
	GameObject controller;

	public GameObject deathCam;

	UsernameDisplay username;

	int kills;
	int deaths;

	public bool isTagger = false;
	public bool isAlive = true;


	

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
		TagManager.Instance.tagger = TagManager.Instance.existingPlayerList[taggerIndex];
		PV.RPC(nameof(RPC_NewTagger), PV.Owner, taggerIndex);	
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

	public void SwapTagger(bool state)
	{
		//Debug.LogError(PV.Owner);
		PV.RPC(nameof(RPC_LocalTagger), RpcTarget.All, state);
	}

	[PunRPC]
	void RPC_LocalTagger(bool state)
	{
		isTagger = state;
	}

	public static PlayerManager Find(Player player)
	{
		return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
	}

	[PunRPC]
    void RPC_NewTagger(int taggerIndex)
    {
		//Debug.LogError("Owner:" + PhotonNetwork.LocalPlayer);
		//Debug.LogError("The Tagger is" + p);

		// TAGGER
		Player p = TagManager.Instance.existingPlayerList[taggerIndex];
		Debug.Log("Tagger Name:" + p.NickName + ". Player Name:" + PV.Owner.NickName + ".");
		isTagger = p == PV.Owner;
	}

	#endregion

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Sending data to others
            stream.SendNext(username.text.color);
        }
        else
        {
            // Receiving data from others
            username.text.color = (Color)stream.ReceiveNext();
        }
    }
}