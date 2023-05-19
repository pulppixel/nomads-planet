using Photon.Pun;
using UnityEngine;

public class RpcHolder : MonoBehaviourPun
{
    [PunRPC]
    private void AllBufferedViaServer(PhotonMessageInfo info)
    {
        Debug.LogFormat("AllBufferedViaServer Sender:{0} PhotonView:{1} PhotonView.Owner:{2} Timestamp:{3}|{4}", 
            info.Sender, info.photonView, info.photonView.Owner, info.SentServerTimestamp, info.SentServerTime);
    }
}
