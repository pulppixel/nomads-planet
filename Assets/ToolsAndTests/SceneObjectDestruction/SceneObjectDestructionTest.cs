using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SceneObjectDestructionTest : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [SerializeField]
    private int requiredPlayersNumber = 0;

    private bool done;

    public override void OnJoinedRoom()
    {
        // re parent a scene object, will it be destroyed for other actors already in the room, will it be destroyed for late joiners?
        PhotonView pV = PhotonView.Find(3);
        pV.transform.SetParent(this.transform);

        this.TestLogic();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        this.TestLogic();
    }

    private void TestLogic()
    {
        if (!done && (this.requiredPlayersNumber == 0 || this.requiredPlayersNumber == PhotonNetwork.CurrentRoom.PlayerCount) && PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            // test buffered RPC clean up & arrival time for networked destroyed scene objects
            this.photonView.RPC("BufferedRpcTest", RpcTarget.AllBufferedViaServer);

            // instantiate and re parent a player object, will it be destroyed for other actors already in the room, will it be destroyed for late joiners?
            GameObject go = PhotonNetwork.Instantiate("PlayerObject", Vector3.zero, Quaternion.identity, 0, new object[] { this.photonView.ViewID });
            go.transform.SetParent(this.transform);
            PhotonView pV = go.GetComponent<PhotonView>();
            pV.RPC("AllBufferedViaServer", RpcTarget.AllBufferedViaServer);

            // instantiate and re parent a room object, will it be destroyed for other actors already in the room, will it be destroyed for late joiners?
            go = PhotonNetwork.InstantiateSceneObject("RoomObjectRuntime", Vector3.zero, Quaternion.identity, 0, new object[] { this.photonView.ViewID });
            go.transform.SetParent(this.transform);
            pV = go.GetComponent<PhotonView>();
            pV.RPC("AllBufferedViaServer", RpcTarget.AllBufferedViaServer);

            // re parent another scene object, will it be destroyed for other actors already in the room, will it be destroyed for late joiners?
            pV = PhotonView.Find(4);
            pV.transform.SetParent(this.transform);

            // network destroy scene object with all the nested PVs (at compile time + added at runtime)
            PhotonNetwork.Destroy(this.gameObject);
        }
    }


    [PunRPC]
    private void BufferedRpcTest(PhotonMessageInfo info)
    {
        Debug.LogFormat("BufferedRpcTest Sender={0}", info.Sender);
    }

    // re parenting for remote clients
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!info.photonView.IsMine && info.photonView.ViewID != this.photonView.ViewID)
        {
            info.photonView.transform.SetParent(this.transform);
        }
    }
}
