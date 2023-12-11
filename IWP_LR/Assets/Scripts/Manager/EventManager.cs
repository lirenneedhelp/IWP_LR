using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class EventManager : MonoBehaviour
{
    public const byte TAGGED_EVENT_CODE = 1;
    public const byte NERF_RUNNERS = 2;
    public const byte NERF_TAGGERS = 3;
    
    public static void AnnounceTaggedPlayer(int playerID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] eventData = new object[] { playerID };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(TAGGED_EVENT_CODE, eventData, raiseEventOptions, SendOptions.SendReliable);
        }

    }
    public static void DebuffRunners()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] eventData = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(NERF_RUNNERS, eventData, raiseEventOptions, SendOptions.SendReliable);
        }
    }

    public static void DebuffTaggers()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] eventData = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(NERF_TAGGERS, eventData, raiseEventOptions, SendOptions.SendReliable);
        }
    }


}
