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

    public bool generated = false;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        existingPlayerList = PhotonNetwork.PlayerList.OrderBy(player => player.NickName).ToList();
    }

    public static int GenerateTagger(int randomSeed)
    {
        if (!Instance.generated)
        {
            Random.InitState(randomSeed);
            List<Player> players = Instance.existingPlayerList;
            int randomTaggerIndex = Random.Range(0, players.Count);
            //Debug.LogError(players.Count);
            //Hashtable customRoomProperties = new();
            //customRoomProperties["Tagger"] = randomTaggerIndex;
            //PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
            Instance.generated = true;
            return randomTaggerIndex;
        }

        return -1;
        // Debug.LogError(randomTaggerIndex);

    }

    public static int SeedGenerator()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        return seed;
    }


}
