using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BufferedRpcRemovalTestPlayer : MonoBehaviourPun
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 100 * this.photonView.Owner.ActorNumber, 200, 30));
            if (GUILayout.Button("Send RPC"))
            {
             this.photonView.RPC("MyRPC", RpcTarget.AllBuffered);
            }
        GUILayout.EndArea();
    }

    [PunRPC]
    public void MyRPC()
    {
        Debug.Log("MyRPC");
    }
}
