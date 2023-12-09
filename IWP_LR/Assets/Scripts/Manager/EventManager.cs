using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class EventManager : MonoBehaviour
{
    public const byte TAGGED_EVENT_CODE = 1;
    
    public static void AnnounceTaggedPlayer(int playerID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] eventData = new object[] { playerID };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(TAGGED_EVENT_CODE, eventData, raiseEventOptions, SendOptions.SendReliable);
        }
    }

}
