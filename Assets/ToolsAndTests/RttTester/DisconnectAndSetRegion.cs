using UnityEngine;

using System.Collections;

using Photon.Pun;
using Photon.Realtime;


public class DisconnectAndSetRegion : MonoBehaviourPunCallbacks
{
    private string regionToUse;

    public void SetUS()
    {
        PhotonNetwork.Disconnect();
        this.regionToUse = "us";
    }

    public void SetRegion(string region)
    {
        if (!string.IsNullOrEmpty(this.regionToUse)) return;

        PhotonNetwork.Disconnect();
        this.regionToUse = region;
    }

	public override void OnDisconnected(DisconnectCause cause)
    {
        if (string.IsNullOrEmpty(this.regionToUse)) return;
        PhotonNetwork.ConnectToRegion(this.regionToUse);
        this.regionToUse = null;
    }
}
