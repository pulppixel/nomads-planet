using Photon.Pun;

public class Subclass : Base {
    
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        if (stream.IsWriting)
        {
            stream.SendNext("hello from subclass");
        }
        else
        {
            string _message = (string)stream.ReceiveNext();
        }
    }


}
