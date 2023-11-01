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
    TMP_Text alive_Text;

    private float currentRoundTime;

    private void Start()
    {
        StartRound();
        UpdatePeopleStatus();
    }

    private void Update()
    {
        
        currentRoundTime -= Time.deltaTime;

        // Update round timer for all players
        UpdateRoundTimer(currentRoundTime);

        // End the round if the timer reaches 0
        if (currentRoundTime <= 0f)
        {
            EndRound();
        }
        
    }

    private void StartRound()
    {
        currentRoundTime = roundDuration;

        // Call RPC to sync round start time with all players
        photonView.RPC(nameof(RPC_StartRound), RpcTarget.All);
    }

    private void EndRound()
    {
        // Perform end of round logic here
        // Start a new round
        StartRound();
    }

    [PunRPC]
    private void RPC_StartRound()
    {
        // Handle round start logic here (if any)
        Debug.Log("Round Started!");
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
        alive_Text.text = "Alive: " + PhotonNetwork.PlayerList.Length;
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
}
