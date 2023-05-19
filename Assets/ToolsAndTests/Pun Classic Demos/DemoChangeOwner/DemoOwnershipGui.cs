using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DemoOwnershipGui : MonoBehaviour, IPunOwnershipCallbacks
{
    public GUISkin Skin;
    public bool TransferOwnershipOnRequest = true;

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget (this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        Debug.Log("OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + targetView + ".");
        if (this.TransferOwnershipOnRequest)
        {
            targetView.TransferOwnership(requestingPlayer.ActorNumber);
        }
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        Debug.Log( "OnOwnershipTransfered for PhotonView "+targetView+" from "+previousOwner+" to "+targetView.Owner);
    }

    /// <inheritdoc />
    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        // implement if you need to react to failed ownership transfer requests
    }


    #region Unity

    public void OnGUI()
    {
        GUI.skin = this.Skin;
        GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
        {
            string label = TransferOwnershipOnRequest ? "passing objects" : "rejecting to pass";
            if (GUILayout.Button(label))
            {
                this.TransferOwnershipOnRequest = !this.TransferOwnershipOnRequest;
            }
        }
        GUILayout.EndArea();
        
        if (PhotonNetwork.InRoom)
        {
   
            int playerNr = PhotonNetwork.LocalPlayer.ActorNumber;
            string playerIsMaster = PhotonNetwork.LocalPlayer.IsMasterClient ? "(master) " : "";
            string playerColor = this.GetColorName(PhotonNetwork.LocalPlayer.ActorNumber);
            GUILayout.Label(string.Format("player {0}, {1} {2}(you)", playerNr, playerColor, playerIsMaster));

            foreach (Player otherPlayer in PhotonNetwork.PlayerListOthers)
            {
                playerNr = otherPlayer.ActorNumber;
                playerIsMaster = otherPlayer.IsMasterClient ? "(master)" : "";
                playerColor = this.GetColorName(otherPlayer.ActorNumber);
                GUILayout.Label(string.Format("player {0}, {1} {2}", playerNr, playerColor, playerIsMaster));
            }

            if (PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length == 0)
            {
                GUILayout.Label("Join more clients to switch object-control.");
            }
        }
        else
        {
            GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        }
    }

    #endregion
    private string GetColorName(int playerId)
    {
        int index = 0;//System.Array.IndexOf(ExitGames.UtilityScripts.PlayerRoomIndexing.instance.PlayerIds, playerId);

        switch (index)
        {
            case 0:
                return "red";
            case 1:
                return "blue";
            case 2:
                return "yellow";
            case 3:
                return "green";
        }

        return string.Empty;
    }


}
