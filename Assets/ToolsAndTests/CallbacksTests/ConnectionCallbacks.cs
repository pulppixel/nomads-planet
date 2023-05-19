using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class ConnectionCallbacks : BaseCallbacks, IConnectionCallbacks
{
    [SerializeField]
    private bool deactivateOnDisconnect;

    #region IConnectionCallbacks

    public void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogFormat("OnDisconnected {0}", cause);
        if (deactivateOnDisconnect)
        {
            this.enabled = false;
        }
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    #endregion
}
