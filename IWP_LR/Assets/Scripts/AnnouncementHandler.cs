using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class AnnouncementHandler : MonoBehaviour, IOnEventCallback
{
    void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        if (photonEvent.Code == EventManager.TAGGED_EVENT_CODE)
        {
            // Extract the tagged player's ID from the event data.
            int taggedPlayerID = (int)photonEvent.CustomData;

            Debug.Log(taggedPlayerID);
            PhotonView taggedPV =  PhotonNetwork.GetPhotonView(taggedPlayerID);

            // Display an announcement or perform any other relevant actions.

            // TO DO: UPDATE PHOTON CHAT 
            Debug.LogError(taggedPV.Owner.NickName + "is 'IT'!");
        }
    }
}

