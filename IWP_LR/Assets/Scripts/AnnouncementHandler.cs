using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class AnnouncementHandler : MonoBehaviour, IOnEventCallback
{
    [SerializeField]
    TMP_Text txt;
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
            object[] eventData = (object[])photonEvent.CustomData;

            // Extract the tagged player's ID from the event data.
            int taggedPlayerID = (int)eventData[0];

            Debug.Log(taggedPlayerID);
            PhotonView taggedPV =  PhotonNetwork.GetPhotonView(taggedPlayerID);

            // Display an announcement or perform any other relevant actions.

            // TO DO: UPDATE PHOTON CHAT
            txt.text = taggedPV.Owner.NickName + " is 'IT'!"; 
            Debug.LogError(taggedPV.Owner.NickName + " is 'IT'!");
        }
    }
}

