using System.Collections;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StringLimitsTest : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private char character = 'a'; //'測';
    
    [SerializeField]
    private int stringLength = short.MaxValue; // ('a') 32767/511986/512076, ('測') 10922/170662/170707

    [SerializeField]
    private byte eventCode = 0;

    [SerializeField]
    private string roomName = "testStringLimitsRoom";

    [SerializeField]
    private SerializationProtocol serializationProtocol;

    [SerializeField]
    private bool broadcast;

    [SerializeField]
    private float interval = 0.1f;

    private Coroutine coroutine;

    private void SendString()
    {
        if (PhotonNetwork.InRoom)
        {
            if (this.broadcast)
            {
                string stringToSend = new string(this.character, this.stringLength);
                Debug.LogFormat("sending string, length={0} size={1}", stringToSend.Length, Encoding.UTF8.GetByteCount(stringToSend));
                PhotonNetwork.RaiseEvent(this.eventCode, stringToSend, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendUnreliable);
            }
            else
            {
                this.coroutine = this.StartCoroutine(SendStringCoroutine());
            }
        }
    }

    private IEnumerator SendStringCoroutine()
    {
        while (true)
        {
            string stringToSend = new string(this.character, this.stringLength);
            Debug.LogFormat("sending string, length={0} size={1}", stringToSend.Length, Encoding.UTF8.GetByteCount(stringToSend));
            PhotonNetwork.RaiseEvent(this.eventCode, stringToSend, new RaiseEventOptions(), SendOptions.SendUnreliable);
            this.stringLength++;
            yield return new WaitForSeconds(interval);
        }
    }
    
    private void Start()
    {
        PhotonNetwork.NetworkingClient.SerializationProtocol = serializationProtocol;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(this.roomName, new RoomOptions(), TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogErrorFormat("OnDisconnected {0}", cause);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public override void OnJoinedRoom()
    {
        this.SendString();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    private void NetworkingClient_EventReceived(EventData eventData)
    {
        if (eventData.Code == this.eventCode)
        {
            string receivedString = eventData.CustomData as string;
            Debug.LogFormat("string received, length={0} size={1}", receivedString.Length, Encoding.UTF8.GetByteCount(receivedString));
            this.stringLength++;
            this.SendString();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        if (this.coroutine != null)
        {
            this.StopCoroutine(this.coroutine);
        }
    }
}