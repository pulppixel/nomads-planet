using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public const byte SEND_UPDATE = 123;

	public GrapherComponent LocalTrianglePrefab;

	public GrapherComponent[] RemoteTrianglePrefabs;

    public GrapherComponent localTriangleInstance { get; set; }

    public List<GrapherComponent> remoteTriangleInstances { get; set; }

    [Range(1.0f, 30.0f)]
	public float SendRate = 10.0f;


	public bool UseLocalAsReference;

	public static NetworkManager Instance;


	public bool _running;

    public void Start()
    {
		Instance = this;

        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("IsConnectedAndReady: " + PhotonNetwork.IsConnectedAndReady);

        if (PhotonNetwork.InRoom)
        {
            GUILayout.Label(PhotonNetwork.CurrentRoom.ToStringFull());
        }

        GUILayout.EndVertical();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Test Room", new RoomOptions(), null);
    }

    public override void OnJoinedRoom()
    {
		remoteTriangleInstances = new List<GrapherComponent> ();

		GameObject _g = Instantiate (LocalTrianglePrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		localTriangleInstance =_g.GetComponent<GrapherComponent>();

		foreach (GrapherComponent _prefab in RemoteTrianglePrefabs)
		{
			_g = Instantiate (_prefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			remoteTriangleInstances.Add(_g.GetComponent<GrapherComponent>());
		}

		_running = true;
    }

    public override void OnLeftRoom()
    {
		Destroy(localTriangleInstance);

		foreach (GrapherComponent _instance in remoteTriangleInstances)
		{
			Destroy (_instance.gameObject);
		}

		remoteTriangleInstances = new List<GrapherComponent> ();

		_running = false;
    }
}