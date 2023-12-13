using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_FontAsset BoldedFont;
    public TMP_InputField messageInput;
    public GameObject chatText;
    public RectTransform content;
    public GameObject chatBox;

    private Coroutine chatBoxCoroutine;

    public int maxMessages = 25;

    public List<Message> messageList = new ();

    private PlayerManager localPlayerManager;

    private void Start()
    {
        localPlayerManager = PlayerManager.Find(PhotonNetwork.LocalPlayer);
    }

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
                chatBox.SetActive(false);
            }

            messageInput.gameObject.SetActive(!messageInput.gameObject.activeInHierarchy);
            Cursor.visible = messageInput.gameObject.activeInHierarchy;
            messageInput.ActivateInputField();
            localPlayerManager.isTyping = !localPlayerManager.isTyping;

            if (messageInput.gameObject.activeInHierarchy)
                chatBox.SetActive(true);




            //inputField.text = string.Empty;
        }
    }

    public void UpdateChat()
    {
        ActivateChatBox();
        // Clear existing chat
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < messageList.Count; ++i)
        {
            GameObject chatTextPrefab = Instantiate(chatText, content);
            TMP_Text chat_text = chatTextPrefab.GetComponent<TMP_Text>();

            if (messageList[i].colourised)
            {
                var temp = chatTextPrefab.AddComponent<TextColourBlink>();
                temp.speed = 5f;
                temp.startColour = messageList[i].start;
                temp.endColour = messageList[i].end;
                chat_text.font = BoldedFont;
            }

            if (messageList[i].start != null)
                chat_text.color = messageList[i].start;

            chat_text.text = messageList[i].text;
            
        }
    }

    // Function to activate the chatBox and start the coroutine
    private void ActivateChatBox()
    {
        chatBox.SetActive(true);

        if (chatBoxCoroutine != null)
        {
            StopCoroutine(chatBoxCoroutine);
        }

        // Start the coroutine to deactivate the chatBox after 5 seconds
        chatBoxCoroutine = StartCoroutine(DeactivateChatBoxAfterDelay(5f));
    }

    // Coroutine to deactivate the chatBox after a specified delay
    private IEnumerator DeactivateChatBoxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Deactivate the chatBox after the delay
        chatBox.SetActive(false);
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public bool colourised;
    public Color start;
    public Color end;
}
