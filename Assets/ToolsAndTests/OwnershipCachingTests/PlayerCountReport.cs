using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountReport : MonoBehaviour {

    Text text;

	// Use this for initialization
	void Awake ()
    {
        text = GetComponentInChildren<Text>();
	}

    private StringBuilder sb = new StringBuilder();

	// Update is called once per frame
	void Update ()
    {
        var room = PhotonNetwork.CurrentRoom;
        if (room == null)
            return;

        int newplayercount = PhotonNetwork.CurrentRoom.PlayerCount;
        var newMaster = PhotonNetwork.MasterClient;

        {
            sb.Length = 0;
            var players = PhotonNetwork.CurrentRoom.Players;
            sb.Append("TTL:").Append(PhotonNetwork.CurrentRoom.PlayerTtl) 
                .Append("\nLocal ").Append((PhotonNetwork.LocalPlayer != null) ? PhotonNetwork.LocalPlayer.ActorNumber.ToString() : "null")
                .Append("\nMaster ").Append((PhotonNetwork.MasterClient !=null) ? PhotonNetwork.MasterClient.ActorNumber.ToString() : "null")
                .Append("\n\nPlayers:");

            foreach (var p in players.Values)
            {
                sb.Append((p.IsInactive ? "\n<color=red>[" : "\n<color=yellow>[") + p.ActorNumber + (p.HasRejoined ? "]</color> Rejoined" : "]</color>"));
            }

            text.text = sb.ToString();


        }


    }
}
