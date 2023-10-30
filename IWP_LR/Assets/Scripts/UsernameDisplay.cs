using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UsernameDisplay : MonoBehaviour
{
	[SerializeField] PhotonView playerPV;
	public TMP_Text text;

	PlayerManager playerManager;



	void Start()
	{
		if(playerPV.IsMine)
		{
			gameObject.SetActive(false);
		}

		text.text = playerPV.Owner.NickName;
		text.color = TagManager.Instance.tagger == playerPV.Owner ? Color.red : Color.green;
        playerManager = PlayerManager.Find(playerPV.Owner);
	
	}
	void Update()
	{
		text.color = playerManager.isTagger ? Color.red : Color.green;
	}

	
}
