using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DebugDisconnects : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause != DisconnectCause.ClientTimeout)
        {
            Debug.LogErrorFormat("OnDisconnected, cause = {0}", cause);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCode.ErrorInfo)
        {
            Debug.LogErrorFormat("ErrorInfo event: {0}", photonEvent.ToStringFull());
        }
    }
}