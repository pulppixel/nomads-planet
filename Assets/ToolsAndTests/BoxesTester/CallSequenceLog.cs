using Photon.Pun;
using UnityEngine;


// Call sequence (on SetActive()):
// Awake
// OnEnable
// Start
public class CallSequenceLog : MonoBehaviour
{
    void Awake()
    {
        PhotonView pv = this.gameObject.GetPhotonView();
        Debug.Log("Awake() " + pv);
    }

    void OnEnable()
    {
        PhotonView pv = this.gameObject.GetPhotonView();
        Debug.Log("OnEnable() " + pv);
    }

    void Start()
    {
        PhotonView pv = this.gameObject.GetPhotonView();
        Debug.Log("Start() " + pv);
    }

    void Update()
    {

    }

    void OnDisable()
    {
        PhotonView pv = this.gameObject.GetPhotonView();
        Debug.Log("OnDisable() " + pv);
    }

    void OnDestroy()
    {
        PhotonView pv = this.gameObject.GetPhotonView();
        Debug.Log("OnDestroy() "+pv);
    }
}