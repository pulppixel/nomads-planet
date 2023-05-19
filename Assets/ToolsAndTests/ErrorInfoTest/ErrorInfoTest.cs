using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ErrorInfoTest : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField]
    private bool testWebhooks; // example AppId 29159b2f-d9e1-4fa8-bf14-539ff15dd737, ErrorInfo = true, IsPersistent = false, set any BaseUrl / paths
    [SerializeField]
    private int eventsBatchSize = 10;
    [SerializeField]
    private byte eventCode = 1;
    [SerializeField]
    private string roomName = "test";
    [SerializeField]
    private int emptyRoomTTL = 60000;
    [SerializeField]
    private bool globalCache = true;

    // Use this for initialization
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private readonly WebFlags webFlags = new WebFlags(WebFlags.HttpForwardConst);

    private readonly RaiseEventOptions addToCacheOptions = new RaiseEventOptions
    {
        Receivers = ReceiverGroup.All
    };

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { EmptyRoomTtl = this.emptyRoomTTL }, TypedLobby.Default);
    }

    public override void OnErrorInfo(ErrorInfo info)
    {
        Debug.LogError(info);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            this.EventsCacheBatch();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == this.eventCode)
        {
            int i = (int) photonEvent.CustomData;
            Debug.Log(i);
            this.EventsCacheBatch(i * this.eventsBatchSize + 1);
        }
    }

    private void EventsCacheBatch(int i = 1)
    {
        RaiseEventOptions reo = this.addToCacheOptions;
        if (this.testWebhooks)
        {
            reo.Flags = webFlags;
        }
        else
        {
            reo.Flags = WebFlags.Default;
        }
        if (this.globalCache)
        {
            reo.CachingOption = EventCaching.AddToRoomCacheGlobal;
        }
        else
        {
            reo.CachingOption = EventCaching.AddToRoomCache;
        }
        for (int j = i; j < i + this.eventsBatchSize; j++)
        {
            PhotonNetwork.RaiseEvent(this.eventCode, j, reo, SendOptions.SendReliable);
        }
    }
}
