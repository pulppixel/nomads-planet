using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BufferedRpcRemovalTest : MonoBehaviour
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

        if (PhotonNetwork.IsMasterClient)
        {
            if (GUILayout.Button("Remove RPCs"))
            {
                foreach (var view in PhotonNetwork.PhotonViews)
                {
                    PhotonNetwork.RemoveRPCs(view);
                    Debug.Log("<color=blue>Removed View RPC </color>" + view.gameObject.name + " " + view.ViewID);
                }
            }
        }

    }
}
