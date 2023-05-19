using Photon.Pun;
using UnityEngine;


public class TestSelfDestructTimer : MonoBehaviour
{
    public int SelfDestructTime = 4;

    public void SelfDestroy()
    {
        //Debug.LogWarning("SelfDestroy", this);

        if (this.gameObject.GetPhotonView().IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public void OnEnable()
    {
        bool isMineNow = this.gameObject.GetPhotonView().IsMine;
        Rigidbody rb = this.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = !isMineNow;
        }

        if (isMineNow)
        {
            this.Invoke("SelfDestroy", SelfDestructTime);
        }
        else
        {
            //Debug.LogWarning("OnEnable for not-mine object "+ this.gameObject.GetPhotonView(), this.gameObject);
        }
    }
}