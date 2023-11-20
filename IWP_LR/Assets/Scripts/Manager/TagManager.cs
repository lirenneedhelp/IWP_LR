using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;



public class TagManager : MonoBehaviour
{
    public static TagManager Instance = null;

    public List<Player> existingPlayerList;
    public Player tagger;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        existingPlayerList = PhotonNetwork.PlayerList.OrderBy(player => player.NickName).ToList();
    }

    public static void GenerateTagger(int randomSeed)
    {
        Random.InitState(randomSeed);
        List<Player> players = Instance.existingPlayerList;

        int randomTaggerIndex = Random.Range(0, players.Count);
        Hashtable customRoomProperties = new ();
        customRoomProperties["Tagger"] = randomTaggerIndex;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        
        Debug.LogError(randomTaggerIndex);

    }


}
