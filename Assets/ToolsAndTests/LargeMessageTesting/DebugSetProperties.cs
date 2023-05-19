using UnityEngine;
using Photon.Pun;


using Hashtable = ExitGames.Client.Photon.Hashtable;


public class DebugSetProperties : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        this.SetProperties();
    }

    private void SetProperties()
    {
        Hashtable props = new Hashtable();
        props[PhotonNetwork.LocalPlayer.ActorNumber.ToString()] = new byte[1024*256];
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}
