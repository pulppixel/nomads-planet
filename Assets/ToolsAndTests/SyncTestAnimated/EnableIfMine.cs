using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;


public class EnableIfMine : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public List<Behaviour> Components;

	
    public override void OnJoinedRoom()
    {
        if (this.photonView.IsMine)
        {
            foreach (Behaviour c in this.Components)
            {
                c.enabled = true;
            }
        }
    }
}
