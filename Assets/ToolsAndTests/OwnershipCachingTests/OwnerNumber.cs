using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OwnerNumber : MonoBehaviour {

    PhotonView pv;

    Text text;

    private void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
        text = GetComponentInChildren<Text>();

        text.text = "Plyr " + pv.OwnerActorNr;
    }


    //public void Update()
    //{

    //    if (!PhotonNetwork.IsConnectedAndReady)
    //    {
    //        Debug.LogError("DESTROY on disconn.");
    //        Destroy(gameObject);

    //    }
    //}


}
