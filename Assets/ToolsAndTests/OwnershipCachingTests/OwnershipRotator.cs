using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipRotator : MonoBehaviourPun
{
    public float rate = 1;
    public Vector3 degreesPerSec = new Vector3(0, 0, 180);


    private void FixedUpdate()
    {
        if (photonView.IsMine)
            transform.eulerAngles = degreesPerSec * Mathf.Sin(Time.time * rate);
    }
    // Update is called once per frame
    void Update ()
    {
        if (photonView.IsMine)
            transform.eulerAngles = degreesPerSec * Mathf.Sin(Time.time * rate);
    }
}
