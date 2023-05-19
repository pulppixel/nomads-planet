using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class OfflineTest : MonoBehaviourPunCallbacks
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());

        Debug.Log("room infos" + PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull());
    }

    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate for"+ target+ " props:"+ changedProps.ToStringFull());

        Debug.Log("player props" + PhotonNetwork.LocalPlayer.CustomProperties.ToStringFull());
    }
    private void OnGUI()
    {
        if (PhotonNetwork.OfflineMode)
        {
            if (GUILayout.Button("Turn Offline OFF"))
            {
                PhotonNetwork.OfflineMode = false;
            }
        }else{
            if (GUILayout.Button("Turn Offline ON"))
            {
                PhotonNetwork.OfflineMode = true;
            }
        }



       if (PhotonNetwork.InRoom)
        {
            if (GUILayout.Button("Set room custom properties"))
            {
                Hashtable props = new Hashtable
                    {
                        {"MyKey", Random.Range(1f,100f)}
                    };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }


        if (GUILayout.Button("Set player properties"))
        {
            Hashtable props = new Hashtable
                    {
                        {"MyKey", Random.Range(1f,100f)}
                    };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
}
