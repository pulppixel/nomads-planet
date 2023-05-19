using Photon.Pun;
using UnityEngine;

public class Base : MonoBehaviour, IPunObservable  {

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext("hello from base");
        }
        else
        {
            string _message = (string)stream.ReceiveNext();
        }
    }
}
