using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonNetworkLegacy : MonoBehaviour, ILobbyCallbacks, IConnectionCallbacks
{
    // http://blogs.unity3d.com/2014/05/16/custom-operator-should-we-keep-it/
    private static bool instantiated;
    private static PhotonNetworkLegacy instance;

    public static PhotonNetworkLegacy Instance
    {
        get
        {
            if (!instantiated)
            {
                SetInstance();
            }
            return instance;
        }
    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    //private static void InitializeInstance()
    //{
    //    SetInstance();
    //}

    private static void SetInstance()
    {
        GameObject singleton = new GameObject();
        singleton.name = string.Format("{0} [Singleton]", typeof(PhotonNetworkLegacy));
        SetInstance(singleton.AddComponent<PhotonNetworkLegacy>());
    }

    private static void SetInstance(PhotonNetworkLegacy pnl)
    {
        instance = pnl;
        DontDestroyOnLoad(instance);
        instantiated = true;
    }

    private Dictionary<string, RoomInfo> cachedRoomList;

    private void Awake()
    {
        if (!instantiated)
        {
            SetInstance(this);
        }
        else if (Instance.GetInstanceID() != this.GetInstanceID())
        {
            Debug.LogWarningFormat("[Singleton] Deleting extra '{0}' instance attached to '{1}'", typeof(PhotonNetworkLegacy), name);
            Destroy(this);
            return;
        }
        cachedRoomList = new Dictionary<string, RoomInfo>();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    public static RoomInfo[] GetRoomsList()
    {
        return Instance.cachedRoomList.Values.ToArray();
    }

    void ILobbyCallbacks.OnJoinedLobby()
    {
    }

    void ILobbyCallbacks.OnLeftLobby()
    {
        this.cachedRoomList.Clear();
    }

    void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    void IConnectionCallbacks.OnConnected()
    {
    }

    void IConnectionCallbacks.OnConnectedToMaster()
    {
    }

    void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
    {
        this.cachedRoomList.Clear();
    }

    void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage)
    {
    }
}
