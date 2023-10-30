using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class DelayStartWaitingRoomController : MonoBehaviourPunCallbacks
{

    /* This object must be attached to an object
    / in the waiting room scene of your project. */

    // photon view for sending rpc that updates the timer
    private PhotonView photonView;
    
    // scene navigation indexes
    [SerializeField]
    private int multiplayerSceneIndex;
    [SerializeField]
    private int menuSceneIndex;
    // number of players in the room out of the total room size
    private int playerCount;
    private int roomSize;
    [SerializeField]
    private int minPlayersToStart;


    //Timer text variables
    [SerializeField]
    private TMP_Text roomCountDisplay;
    [SerializeField]
    private TMP_Text timerToStartDisplay;

    // bool values for if the timer can count down
    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;
    //countdown timer variables
    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;
    //countdown timer reset variables
    [SerializeField]
    private float maxWaitTime;
    [SerializeField]
    private float maxFullGameWaitTime;

    // Start is called before the first frame update
    void Start()
    {
        //initialise variables
        photonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        PlayerCountUpdate();
    }

    private void PlayerCountUpdate()
    {
        // Updates player count when players join the room

        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomCountDisplay.text = "No. Of Players:" + playerCount + "/" + roomSize;

        if (playerCount == roomSize)
        {
            readyToStart = true;
        }
        else if (playerCount >= minPlayersToStart)
        {
            readyToCountDown = true;
        }
        else
        {
            readyToCountDown = false;
            readyToStart = false;
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Called whenever a new player joins the room
        PlayerCountUpdate();
        //send master clients countdown timer to all other players in order to sync time.
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("RPC_SendTimer",RpcTarget.AllViaServer , timerToStartGame);
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn)
    {
        // RPC for syncing the countdown timer to those that join after it has started the countdown
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if (timeIn < fullGameTimer)
        {
            fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //called whenever player leaves the room
        PlayerCountUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        WaitingForMorePlayers();
    }

    private void WaitingForMorePlayers()
    {
        // If there is only one player in the room the timer will stop and reset
        if (playerCount <= 1)
        {
            ResetTimer();
        }
        // When there is enough players in the room the start timer will begin counting down
        if (readyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if (readyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }

        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerToStartDisplay.text = "Round Starts In:" + tempTimer;
        // if the countdown timer reaches 0 the game will then start
        if (timerToStartGame <= 0f)
        {
            if (startingGame)
                return;
            StartGame();
        }
    }

    void ResetTimer()
    {
        //resets the countdown timer
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    public void StartGame()
    {
        // Multiplayer scene is loaded to start the game
        startingGame = true;
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (PhotonNetwork.CurrentRoom.PlayerCount >= playerCount)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }

    private void DelayCancel()
    {
        // public function paired to cancel button in waiting room scene
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }
}
