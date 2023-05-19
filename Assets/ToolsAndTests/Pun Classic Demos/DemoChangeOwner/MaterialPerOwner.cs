using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;


[RequireComponent( typeof( PhotonView ) )]
public class MaterialPerOwner : MonoBehaviourPun
{
    private int assignedColorForUserId = -1;

    public static Color[] Colors = new Color[] { Color.grey, Color.red, Color.blue, Color.green, Color.yellow, Color.cyan };
    public static string[] ColorNames = new string[] { "grey", "red", "blue", "green", "yellow", "cyan" };


    Renderer m_Renderer;


    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        if (this.photonView.OwnerActorNr != assignedColorForUserId)
        {
            //   int index = System.Array.IndexOf(ExitGames.UtilityScripts.PlayerRoomIndexing.instance.PlayerIds, this.photonView.ownerId);
            try
            {
                this.assignedColorForUserId = this.photonView.OwnerActorNr;
                m_Renderer.material.color = Colors[this.assignedColorForUserId];
            }
            catch (Exception e)
            {
                //nothing
            }

            //Debug.Log("Switched Material to: " + this.assignedColorForUserId + " " + this.renderer.material.GetInstanceID());
        }
    }
}
