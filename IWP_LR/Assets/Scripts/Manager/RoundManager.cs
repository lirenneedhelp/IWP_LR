using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
public class RoundManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public float roundDuration = 300f; // Round duration in seconds (5 minutes in this example)

    [SerializeField]
    TMP_Text text;

    [SerializeField]
    TMP_Text alive_Text, goal_Text;

    [SerializeField]
    GameObject ExplosionVFX;

    [SerializeField]
    GameObject popUpDisplay;

    PlayerManager player;

    private float currentRoundTime;

    private float cooldown = 5f; // Cooldown between rounds

    public int roomSize;

    bool loaded = false;

    bool isStarted = false;

    bool startCountDown = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Write your custom data to the stream
            stream.SendNext(roomSize);
        }
        else
        {
            // Read the custom data from the stream
            roomSize = (int)stream.ReceiveNext();
        }
    }
    private void Start()
    {
        player = PlayerManager.Find(PhotonNetwork.LocalPlayer);
        roomSize = TagManager.Instance.existingPlayerList.Count;

        StartRound();
        UpdatePeopleStatus();

    }

    private void Update()
    {
        if (isStarted)
        {
            currentRoundTime -= Time.deltaTime;
            // Update round timer for all players
            UpdateRoundTimer(currentRoundTime);
            UpdatePeopleStatus();
            UpdateObjective();

            if (currentRoundTime <= 0f)
                EndRound();
        }
        else
        {
            if (startCountDown)
            {
                if (cooldown < 0)
                {
                    // Start a new round
                    StartRound();
                }
                else
                {
                    cooldown -= Time.deltaTime;
                }
            }
        }

    }

    private void StartRound()
    {
        // Call RPC to sync round start time with all players
        photonView.RPC(nameof(RPC_StartRound), RpcTarget.All, 0);
    }

    private void EndRound()
    {
        isStarted = false;

        // Perform end of round logic here
        if (roomSize > 1)
        {
            if (PhotonNetwork.IsMasterClient)
            // ENDS ROUND
            {
                photonView.RPC(nameof(RPC_EndRound), RpcTarget.All);
                //StartRound();
            }

        }
        else
        {
            if (!loaded)
            {
                // Win Lobby
                //if (PhotonNetwork.IsMasterClient)
                    //photonView.RPC(nameof(RPC_EndGame), RpcTarget.All);

                loaded = true;
            }
        }
    }

    private void CallRandomEvent()
    {

    }
    #region RPC_FUNCTIONS

    [PunRPC]
    private void RPC_StartRound(int randomSeed)
    {
        if (PhotonNetwork.IsMasterClient)
        {
           // Debug.LogError("Generating new taggers for new round");
            TagManager.GenerateTagger(randomSeed);
            player.UpdateTaggers();

            
        }

        photonView.RPC(nameof(PopUp), RpcTarget.All);

        currentRoundTime = roundDuration;
        cooldown = 5f;
        isStarted = true;
        Debug.Log("Round Started!");
    }
    // Kill Off Tagger
    [PunRPC]
    private void RPC_EndRound()
    {
        if (player.isTagger)
        {
            photonView.RPC(nameof(RPC_KillSurroundingPlayers), RpcTarget.All, player.controllerPosition);
            photonView.RPC(nameof(RPC_UpdatePlayerCount), RpcTarget.All);
        }

        startCountDown = true;

        if (PhotonNetwork.IsMasterClient)
            TagManager.Instance.generated = true;
    }
    // REMOVES Tagger from the list and updates the display
    [PunRPC]
    private void RPC_UpdatePlayerCount()
    {
        roomSize = TagManager.Instance.existingPlayerList.Count;
    }
    // End Game
    [PunRPC]
    private void RPC_EndGame()
    {
        PhotonNetwork.LoadLevel(2);
    }
    [PunRPC]
    private void RPC_CheckForExistingTaggers()
    {
        foreach (Player p in TagManager.Instance.existingPlayerList)
        {
            if (PlayerManager.Find(p).isTagger)
            {
                return;
            }
        }
        EndRound();
    }
    [PunRPC]
    private void RPC_KillSurroundingPlayers(Vector3 taggerPos)
    {
        Instantiate(ExplosionVFX, taggerPos, Quaternion.identity);

        GameObject[] playerControllers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject GO in playerControllers)
        {
            if (GO == null)
                continue;

            var pm = GO.GetComponent<PlayerController>();

            if (Vector3.Distance(taggerPos, pm.transform.position) < 2f)
            {
                if (pm.playerManager.isAlive)
                {
                    pm.playerManager.Die();
                }
                TagManager.Instance.existingPlayerList.Remove(pm.playerManager.PV.Owner);
                Debug.LogError("Dawg SPLODED!");
            }
        }

    }

    [PunRPC]
    private void PopUp()
    {
        if (player.isTagger && player.PV.IsMine)
            Instantiate(popUpDisplay);
    }


    #endregion

    #region Update Display
    private void UpdateRoundTimer(float time)
    {
        // Calculate minutes and seconds from the remaining time
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        // Update the TMP_Text component with the formatted time
        text.text = "Explosion in: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdatePeopleStatus()
    {
        alive_Text.text = "Alive: " + roomSize;
    }

    private void UpdateObjective()
    {
        if (player.isTagger)
        {
            goal_Text.text = "Goal: Hunt em down!";
            goal_Text.color = Color.red;
        }
        else if (player.isAlive == false)
        {
            goal_Text.text = "Goal: Dead!";
            goal_Text.color = Color.black;
        }
        else
        {
            goal_Text.text = "Goal: Run Away!";
            goal_Text.color = Color.green;
        }
    }
    #endregion



    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (TagManager.Instance.existingPlayerList.Contains(otherPlayer))
            {
                TagManager.Instance.existingPlayerList.Remove(otherPlayer);
                roomSize = TagManager.Instance.existingPlayerList.Count;
            }

            photonView.RPC(nameof(RPC_CheckForExistingTaggers), RpcTarget.All);
        }

    }
    
}
