using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class RoundManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public float roundDuration = 300f; // Round duration in seconds (5 minutes in this example)

    [SerializeField]
    TMP_Text text;

    [SerializeField]
    TMP_Text alive_Text, goal_Text;

    PlayerManager player;

    private float currentRoundTime;

    private float cooldown = 5f; // Cooldown between rounds

    public int roomSize;

    bool loaded = false;

    private void Start()
    {
        StartRound();
        UpdatePeopleStatus();
        player = PlayerManager.Find(PhotonNetwork.LocalPlayer);
        roomSize = PhotonNetwork.PlayerList.Length;
    }

    private void Update()
    {
        currentRoundTime -= Time.deltaTime;

        // Update round timer for all players
        UpdateRoundTimer(currentRoundTime);
        UpdatePeopleStatus();
        UpdateObjective();

        // End the round if the timer reaches 0
        if (currentRoundTime <= 0f && roomSize > 1)
        {
            EndRound();
        }

    }

    private void StartRound()
    {
        currentRoundTime = roundDuration;
        cooldown = 5f;
        // Call RPC to sync round start time with all players
        photonView.RPC(nameof(RPC_StartRound), RpcTarget.All, 0);
    }

    private void EndRound()
    {

        // Perform end of round logic here
        photonView.RPC(nameof(RPC_EndRound), RpcTarget.All);


        if (roomSize > 1)
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
        else
        {
            if (!loaded)
            {
                // Win Lobby
                //PhotonNetwork.LeaveRoom();
                loaded = true;
            }
        }
    }

    [PunRPC]
    private void RPC_StartRound(int randomSeed)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            TagManager.GenerateTagger(randomSeed);
        }

        Debug.Log("Round Started!");
    }
    [PunRPC]
    private void RPC_EndRound()
    {
        if (player.isTagger)
        {
            player.Die();
            photonView.RPC(nameof(RPC_UpdatePlayerCount), RpcTarget.All);
        }
    }
    [PunRPC]
    private void RPC_UpdatePlayerCount()
    {
        roomSize--;
    }

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
        else
        {
            goal_Text.text = "Goal: Run Away!";
            goal_Text.color = Color.green;
        }
    }

    // Implement IPunObservable interface methods for manual synchronization
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // This is the master client; send the current round time to others
            stream.SendNext(currentRoundTime);
        }
        else
        {
            // This is a remote client; receive the round time and update it
            currentRoundTime = (float)stream.ReceiveNext();
        }
    }

    public override void OnLeftRoom()
    {
        if (roomSize <= 1)
            PhotonNetwork.LoadLevel(2);
        roomSize--;

    }
}
