using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField messageInput;
    public TMP_Text chatText;

    public void SendMessage()
    {
        string message = messageInput.text;
        if (!string.IsNullOrEmpty(message))
        {
            photonView.RPC("ReceiveMessage", RpcTarget.All, PhotonNetwork.NickName, message);
            messageInput.text = "";
        }
    }

    [PunRPC]
    void ReceiveMessage(string sender, string message)
    {
        chatText.text += $"{sender}: {message}\n";
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
}
