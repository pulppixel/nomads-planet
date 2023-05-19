using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

public class OnEventCallback : BaseCallbacks, IOnEventCallback
{
    public void OnEvent(EventData photonEvent)
    {
        Debug.Log(photonEvent.ToStringFull());
    }
}
