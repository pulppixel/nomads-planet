using Photon.Pun;
using UnityEngine;

public class LeaveAndRejoin : MonoBehaviourPunCallbacks
{
    public GameObject spawnablePrefab;

    public static bool hasSpawnedRoomObject;

    private void Awake()
    {
        hasSpawnedRoomObject = false;
    }

    public override void OnJoinedRoom()
    {
        
        if (PhotonNetwork.IsMasterClient && !hasSpawnedRoomObject)
        {
            var localPlayer = PhotonNetwork.LocalPlayer;
            var go = PhotonNetwork.InstantiateRoomObject(spawnablePrefab.name, new Vector3(-6, -3, 0), new Quaternion());
            go.name = "Spawned Room Object";
            hasSpawnedRoomObject = true;
        }
    }

    void Update()
    {
        // Force quit without any shutdown messages
        if (Input.GetKeyDown(KeyCode.D))
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.StopThread();

        if (Input.GetKeyDown(KeyCode.C))
            PhotonNetwork.ReconnectAndRejoin();

        if (Input.GetKeyDown(KeyCode.L))
            PhotonNetwork.Disconnect();

        if (PhotonNetwork.InRoom)
        {
           
            if (Input.GetKeyDown(KeyCode.M))
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);

            // Prevent later master changes from spawning objects OnJoin, and rejoins by master from spawning the test room oject repeatedly
            hasSpawnedRoomObject = true;
        }

        if (Input.GetKeyDown(KeyCode.Q))
            foreach (var view in PhotonNetwork.PhotonViewCollection)
                view.RequestOwnership();


        if (Input.GetKeyDown(KeyCode.K))
            foreach (var view in PhotonNetwork.PhotonViewCollection)
                PhotonNetwork.Destroy(view);

    }
}
