using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace emotitron.Networking
{
    public class OwnershipRoomObject : MonoBehaviour
    {

        PhotonView pv;

        Text text;

        int prevownerid, prevcontrollerid;

        private void Awake()
        {
            pv = GetComponentInParent<PhotonView>();
            text = GetComponentInChildren<Text>();

        }

        void Update()
        {


            if (Input.GetKeyDown(KeyCode.T))
                pv.TransferOwnership(PhotonNetwork.LocalPlayer);

            if (Input.GetKeyDown(KeyCode.R))
                pv.TransferOwnership(0);
           

            int newownerid = pv.OwnerActorNr;
            int newctrid = pv.ControllerActorNr;

            prevownerid = newownerid;
            prevcontrollerid = newctrid;

            text.text = pv.ViewID + " " + pv.OwnershipTransfer + (PhotonNetwork.InRoom ? "\n<color=green> " : "\n<color=red> ") + 
                newownerid + ":" + newctrid + "</color>";

        }
    }
}
