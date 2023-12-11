using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class AnnouncementHandler : MonoBehaviour, IOnEventCallback
{
    [SerializeField]
    ChatManager chatManager;
    [SerializeField]
    GameObject nerfRunnerPopUp;

    void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
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
            Message newMessage = new();
            newMessage.text = taggedPV.Owner.NickName + " is 'IT'!";

            if (chatManager.messageList.Count >= chatManager.maxMessages)
                chatManager.messageList.Remove(chatManager.messageList[0]);
            chatManager.messageList.Add(newMessage);
            chatManager.UpdateChat();
            Debug.LogError(taggedPV.Owner.NickName + " is 'IT'!");
        }
        else if (photonEvent.Code == EventManager.NERF_RUNNERS)
        {
            PlayerManager localPlayer = PlayerManager.Find(PhotonNetwork.LocalPlayer);
            if (!localPlayer.isTagger)
            {
                Instantiate(nerfRunnerPopUp);
                localPlayer.controller.GetComponent<PlayerController>().ApplyDebuff(8f);
            }
        }
    }
}

