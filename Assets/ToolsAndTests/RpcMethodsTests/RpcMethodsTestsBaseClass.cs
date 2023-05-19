using Photon.Pun;

public class RpcMethodsTestsBaseClass : MonoBehaviourPunCallbacks 
{
    public virtual void BaseMethodWithoutPunRpcAttribute()
    {
    }

    [PunRPC]
    public virtual void BaseMethodWithPunRpcAttribute()
    {
    }

    [PunRPC]
    protected void BaseMethod()
    {

    }
}
