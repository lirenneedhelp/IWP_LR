using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;


public class PingChecker : MonoBehaviour
{
    [SerializeField]
    TMP_Text ping_text  = null;
    // Start is called before the first frame update
    void Start()
    {
        ping_text.text = "Ping:" + PhotonNetwork.GetPing();
    }

    // Update is called once per frame
    void Update()
    {
        ping_text.text = "Ping:" + PhotonNetwork.GetPing();
    }
}
