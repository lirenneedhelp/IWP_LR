using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class TagManager : MonoBehaviour
{
    public static TagManager Instance = null;

    public Player tagger;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public static void GenerateTagger(int randomSeed)
    {
        Random.InitState(randomSeed);
        Player[] players = PhotonNetwork.PlayerList;

        int randomTaggerIndex = Random.Range(0, players.Length);
        Hashtable customRoomProperties = new ();
        customRoomProperties["Tagger"] = randomTaggerIndex;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        
        Debug.Log(randomTaggerIndex);

    }


}
