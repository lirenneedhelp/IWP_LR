using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DelayStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject delayStartButton;
    [SerializeField]
    private GameObject delayCancelButton;
    [SerializeField]
    private int roomSize;

    public override void OnConnectedToMaster() // Callback function for when the first connection is established
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        delayStartButton.SetActive(true);
    }
    public void DelayStart() // Paired to the Delay Start button
    {
        delayStartButton.SetActive(false);
        delayCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); // First tries to join an existing room
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom(); // If it fails to join a room then it will try to create its own
    }

    void CreateRoom()
    {
        Debug.Log("Creating room");
        int randRoomNo = Random.Range(0,10000); // Creating a random name for the room
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom("Room" + randRoomNo, roomOps); //attempting to create a new room
        Debug.Log(randRoomNo);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... trying again");
        CreateRoom(); // Retrying to create a new room with a different name 
    }

    public void DelayCancel() // Paired to the cancel button. Used to stop looking for a room to join
    {
        delayCancelButton.SetActive(false);
        delayStartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }


}
