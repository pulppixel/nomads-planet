using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class OnApplicationQuitSend : MonoBehaviour, IOnEventCallback
{

    public Text UiText;

	// Use this for initialization
	void Start () {
		PhotonNetwork.AddCallbackTarget(this);
	}


    public void OnApplicationQuit()
    {
        PhotonNetwork.RaiseEvent(10, "I quit.", RaiseEventOptions.Default, SendOptions.SendUnreliable);
        PhotonNetwork.SendAllOutgoingCommands();

        this.quitted = true;
    }

    public bool quitted;
    public void OnDisable()
    {
        Debug.Log("OnDisable after quit: "+this.quitted);
    }


    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("Event: " + photonEvent);
        if (this.UiText != null)
        {
            this.UiText.text += photonEvent.ToStringFull() + "\n";
        }
    }
}
