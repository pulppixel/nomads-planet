using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipColor : MonoBehaviour {

    PhotonView pv;
    MeshRenderer mr;

    private static bool showController = true;
    private static int toggleFrame;

    private void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
        mr = GetComponent<MeshRenderer>();
    }

    
    public static Color GetColor(int colorChoice)
    {
        switch (colorChoice)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.grey;
            case 6: return Color.magenta;
            case 7: return Color.white;
        }

        return Color.black;
    }


    void Update ()
    {
        if (Input.GetKey(KeyCode.Tab) && Input.GetKeyDown(KeyCode.Space) && toggleFrame != Time.frameCount)
        {
            showController = !showController;
            toggleFrame = Time.frameCount;
        }


        if (PhotonNetwork.InRoom)
        {
            if (mr.materials.Length > 1)
            {
                mr.materials[0].color = GetColor(this.pv.ControllerActorNr);
                mr.materials[1].color = GetColor(this.pv.OwnerActorNr);
            }
            else {
                if (showController) mr.material.color = GetColor(this.pv.ControllerActorNr);
                else mr.material.color = GetColor(this.pv.OwnerActorNr);
            }
        }
        else
        {
            mr.material.color = Color.red;
        }
    }
}
