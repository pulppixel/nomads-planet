using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;


public class ChatConnect : MonoBehaviour, IChatClientListener
{
    public ChatClient ChatClient;

    // Use this for initialization
    void Start()
    {
        var settings = PhotonNetwork.PhotonServerSettings.AppSettings;
        this.ChatClient = new ChatClient(this);
        bool connecting = this.ChatClient.Connect(settings.AppIdChat, settings.AppVersion, new AuthenticationValues());
        ChatConnectionHandler handler = this.gameObject.AddComponent<ChatConnectionHandler>();
        handler.Client = this.ChatClient;

        if (!connecting)
        {
            Debug.Log("Failed in Connect()");
        }
    }


    void Update()
    {
        this.ChatClient.Service();
    }


    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(level + " " + message);
    }

    public void OnDisconnected()
    {
    }

    public void OnConnected()
    {
        Debug.Log("Chat OnConnected()");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat OnChatStateChange(" + state + ")");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }

    #if CHAT_EXTENDED

    public void OnChannelPropertiesChanged(string channel, string userId, System.Collections.Generic.Dictionary<object, object> properties)
    {
    }

    public void OnUserPropertiesChanged(string channel, string targetUserId, string senderUserId, System.Collections.Generic.Dictionary<object, object> properties)
    {
    }

    public void OnErrorInfo(string channel, string error, object data)
    {
    }

    #endif
}