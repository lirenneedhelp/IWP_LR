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
    GameObject nerfRunnerPopUp, nerfTaggerPopUp;

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
            newMessage.start = Color.red;

            if (chatManager.messageList.Count >= chatManager.maxMessages)
                chatManager.messageList.Remove(chatManager.messageList[0]);
            chatManager.messageList.Add(newMessage);
            chatManager.UpdateChat();
            Debug.LogError(taggedPV.Owner.NickName + " is 'IT'!");
        }
        else if (photonEvent.Code == EventManager.NERF_RUNNERS)
        {
            PlayerManager localPlayer = PlayerManager.Find(PhotonNetwork.LocalPlayer);
            Message newMessage = new();
            newMessage.text = "Runners do be looking slow, dont cha think so :D";
            newMessage.colourised = true;
            newMessage.start = Color.red;
            newMessage.end = Color.black;

            if (chatManager.messageList.Count >= chatManager.maxMessages)
                chatManager.messageList.Remove(chatManager.messageList[0]);
            chatManager.messageList.Add(newMessage);
            chatManager.UpdateChat();

            if (!localPlayer.isTagger && localPlayer.isAlive)
            {
                Instantiate(nerfRunnerPopUp);
                localPlayer.controller.GetComponent<PlayerController>().ApplyDebuff(8f);
            }
        }
        else if (photonEvent.Code == EventManager.NERF_TAGGERS)
        {
            PlayerManager localPlayer = PlayerManager.Find(PhotonNetwork.LocalPlayer);
            Message newMessage = new();
            newMessage.text = "Aye, the taggers may be hitting a roadblock!";
            newMessage.colourised = true;
            newMessage.start = Color.blue;
            newMessage.end = Color.green;

            if (chatManager.messageList.Count >= chatManager.maxMessages)
                chatManager.messageList.Remove(chatManager.messageList[0]);
            chatManager.messageList.Add(newMessage);
            chatManager.UpdateChat();

            if (localPlayer.isTagger && localPlayer.isAlive)
            {
                Instantiate(nerfTaggerPopUp);
                localPlayer.controller.GetComponent<PlayerController>().ApplyDebuff(8f);
            }
        }
    }
}

