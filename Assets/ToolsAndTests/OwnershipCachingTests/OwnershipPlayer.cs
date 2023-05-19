using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace emotitron.Networking
{
    public class OwnershipPlayer : MonoBehaviourPunCallbacks
    {

        public static PhotonView LocalPlayer;

        //private void OnDestroy()
        //{
        //    Debug.Log("Destroy  OwnershipPlayer " + name);
        //}

        void Start()
        {
            var pv = GetComponent<PhotonView>();

            if (pv.IsMine)
                LocalPlayer = pv;

            //Debug.Log("Set PV creator: " + pv.CreatorActorNr + " mine: " + pv.IsMine + " locaplyer notnull?" + (LocalPlayer != null));

            if (pv.CreatorActorNr == 2)
            {
                var sceneObj = FindObjectOfType<OwnershipRoomObject>();

                if (sceneObj.transform.parent == null)
                    sceneObj.transform.SetParent(transform);

                //Debug.Log("Grab scene obj: " + sceneObj.name + " -> "  + sceneObj.transform.parent.name);

            }

        }
    }
}
