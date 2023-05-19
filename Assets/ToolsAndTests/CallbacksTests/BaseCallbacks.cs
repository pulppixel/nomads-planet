using Photon.Pun;
using UnityEngine;

public abstract class BaseCallbacks : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    protected virtual void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
