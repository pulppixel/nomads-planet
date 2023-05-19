using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;


public class CachedEventCounter : MonoBehaviour
{
    public Text OutputText;

    private readonly Dictionary<byte, int> eventCount = new Dictionary<byte, int>();

	void Start ()
	{
	    PhotonNetwork.NetworkingClient.EventReceived += this.OnEvent;
	}

    
    // Registered for PhotonNetwork.NetworkingClient.EventReceived
    private void OnEvent(EventData ev)
    {
        if (this.eventCount.ContainsKey(ev.Code))
        {
            this.eventCount[ev.Code]++;
        }
        else
        {
            this.eventCount[ev.Code] = 1;
        }

        if (this.OutputText != null)
        {
            this.OutputText.text = this.ToString();
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<byte, int> pair in this.eventCount)
        {
            sb.AppendFormat("{0}={1}x, ", pair.Key, pair.Value);
        }

        return sb.ToString();
    }
}
