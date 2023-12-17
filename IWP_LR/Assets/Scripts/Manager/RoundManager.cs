using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
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

    float[] timeRanges = { 1.5f, 2, 3 };

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
            if (currentRoundTime >= 0)
                UpdateRoundTimer(currentRoundTime);

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
                    if (roomSize > 1)
                        StartRound();
                    else
                    {
                        Debug.Log("Winner Is" + TagManager.Instance.existingPlayerList[0].NickName);
                        if (!loaded)
                        {
                            photonView.RPC(nameof(RPC_EndGame), RpcTarget.All);
                            loaded = true;
                        }

                    }
                }
                else
                {
                    cooldown -= Time.deltaTime;
                }
            }
        }

        UpdatePeopleStatus();
        UpdateObjective();

    }

    private void StartRound()
    {
        // Call RPC to sync round start time with all players
        if (PhotonNetwork.IsMasterClient)
        {
            int seed = TagManager.SeedGenerator();
            photonView.RPC(nameof(RPC_StartRound), RpcTarget.AllViaServer, seed);
        }
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
       
    }

    private void CallRandomEvent()
    {
        int randomEvent = Random.Range(0, 2);
        photonView.RPC(nameof(InvokeEvent), RpcTarget.All, randomEvent);

    }
    #region RPC_FUNCTIONS

    [PunRPC]
    private void RPC_StartRound(int randomSeed)
    {
        currentRoundTime = CalculateTimer() ;
        cooldown = 5f;
        isStarted = true;

        if (PhotonNetwork.IsMasterClient)
        {
           // Debug.LogError("Generating new taggers for new round");
            //TagManager.GenerateTagger(randomSeed);
            player.UpdateTaggers(TagManager.GenerateTagger(randomSeed));
            StartCoroutine(CountdownEvent(currentRoundTime * 0.5f));
        }

        photonView.RPC(nameof(PopUp), RpcTarget.All);
        


    }
    // Kill Off Tagger
    [PunRPC]
    private void RPC_EndRound()
    {
        if (player.isTagger)
        {
            photonView.RPC(nameof(RPC_KillSurroundingPlayers), RpcTarget.AllViaServer, player.controllerPosition);
            photonView.RPC(nameof(RPC_UpdatePlayerCount), RpcTarget.AllViaServer);
        }

        startCountDown = true;

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
        ToggleMouse.OnCursor();
        PhotonNetwork.LoadLevel(0);
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
        if (player.isTagger && player.isAlive)
            Instantiate(popUpDisplay);
    }

    [PunRPC]

    private void InvokeEvent(int eventNumber)
    {
        switch (eventNumber)
        {
            case 0:
                EventManager.DebuffRunners();
                break;
            case 1:
                EventManager.DebuffTaggers();
                break;
        }
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

    public float CalculateTimer()
    {
        int count = TagManager.Instance.existingPlayerList.Count;

        float divide = 1;
        if (count > 10)
            divide = timeRanges[0];
        else if (count > 6)
            divide = timeRanges[1];
        else if (count > 0)
            divide = timeRanges[2];

        return roundDuration;
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
    private IEnumerator CountdownEvent(float midRoundDuration)
    {
        yield return new WaitForSeconds(midRoundDuration);

        CallRandomEvent();
    }


}
