using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField messageInput;
    public GameObject chatText;
    public RectTransform content;

    
    public int maxMessages = 25;

    public List<Message> messageList = new ();

    public void SendMessage()
    {
        string message = messageInput.text;
        if (!string.IsNullOrEmpty(message))
        {
            photonView.RPC(nameof(ReceiveMessage), RpcTarget.All, PhotonNetwork.NickName, message);
            messageInput.text = "";
        }
    }

    [PunRPC]
    void ReceiveMessage(string sender, string message)
    {
        if (messageList.Count >= maxMessages)
            messageList.Remove(messageList[0]);

        Message newMessage = new ();
        newMessage.text = $"{sender}: {message}\n";
        messageList.Add(newMessage);
        //chatText.text += $"{sender}: {message}\n";
        UpdateChat();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            
            if (messageInput.gameObject.activeInHierarchy)
            {
                SendMessage();
                Cursor.lockState = CursorLockMode.Locked;
            }

            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            messageInput.gameObject.SetActive(!messageInput.gameObject.activeInHierarchy);
            Cursor.visible = messageInput.gameObject.activeInHierarchy;
            messageInput.ActivateInputField();



            //inputField.text = string.Empty;
        }
    }

    public void UpdateChat()
    {
        // Clear existing chat
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < messageList.Count; ++i)
        {
            GameObject chatTextPrefab = Instantiate(chatText, content);
            TMP_Text chat_text = chatTextPrefab.GetComponent<TMP_Text>();
            chat_text.text = messageList[i].text;
        }
    }
}

[System.Serializable]
public class Message
{
    public string text;
}
