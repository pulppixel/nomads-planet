using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomObjectBufferedRpcCleanUpTest : MonoBehaviourPunCallbacks
{
    [SerializeField] 
    private Text clientStateText;

    [SerializeField]
    private Button joinButton, leaveButton, instantiateButton, rpcButton, removeRpcsButton, destroyButton;

    [SerializeField]
    private string roomName = "test";

    [SerializeField]
    private string prefabName = "SceneObjectPrefab";

    [SerializeField]
    private string rpcMethodName = "AllBufferedViaServer";

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.StateChanged += NetworkingClientOnStateChanged;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.StateChanged -= NetworkingClientOnStateChanged;
        base.OnDisable();
    }

    private void NetworkingClientOnStateChanged(ClientState previousState, ClientState currentState)
    {
        if (this.clientStateText && this.clientStateText != null)
        {
            this.clientStateText.text = Enum.GetName(typeof(ClientState), currentState);
        }
    }

    public PhotonView LastSceneView
    {
        get
        {
            if (PhotonNetwork.InRoom)
            {
                for (int i = 1; i < PhotonNetwork.MAX_VIEW_IDS; i++)
                {
                    PhotonView view = PhotonView.Find(i);
                    if (view != null)
                    {
                        return view;
                    }
                }

            }
            return null;
        }
    }

    private void Start()
    {
        joinButton.onClick.AddListener(this.JoinRoom);
        leaveButton.onClick.AddListener(this.LeaveRoom);
        instantiateButton.onClick.AddListener(this.InstantiateRoomObject);
        rpcButton.onClick.AddListener(this.CallRpc);
        removeRpcsButton.onClick.AddListener(this.RemoveRpcs);
        destroyButton.onClick.AddListener(this.DestroyRoomObject);
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        joinButton.gameObject.SetActive(PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
        leaveButton.gameObject.SetActive(PhotonNetwork.InRoom);
        bool instantiated = LastSceneView != null;
        bool canInstantiate = PhotonNetwork.InRoom && !instantiated;
        instantiateButton.gameObject.SetActive(canInstantiate);
        rpcButton.gameObject.SetActive(instantiated);
        removeRpcsButton.gameObject.SetActive(instantiated);
        destroyButton.gameObject.SetActive(instantiated);
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(this.roomName, new RoomOptions(), TypedLobby.Default);
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    private void CallRpc()
    {
        LastSceneView.RPC(this.rpcMethodName, RpcTarget.AllBufferedViaServer);
    }

    private void InstantiateRoomObject()
    {
        PhotonNetwork.InstantiateRoomObject(this.prefabName, Vector3.one, Quaternion.identity);
    }

    private void RemoveRpcs()
    {
        PhotonNetwork.RemoveRPCs(LastSceneView);
    }

    private void DestroyRoomObject()
    {
        PhotonNetwork.Destroy(LastSceneView);
    }
}
