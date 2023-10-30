using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TagManager : MonoBehaviour
{
    public static TagManager Instance = null;

    public Photon.Realtime.Player tagger;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        // // Get all players in the room
		// Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

		// // Randomly choose a tagger
		// int randomTaggerIndex = Random.Range(0, players.Length);
		// tagger = players[randomTaggerIndex];
    }
}
