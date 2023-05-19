using Photon.Pun;
using UnityEngine;

public class RpcMethodsTests : RpcMethodsTestsBaseClass
{
    [SerializeField]
    private string methodName;

    [SerializeField] 
    private int paramsNumber;

    private object[] parameters;

    [SerializeField]
    private bool arrayParam;

    [SerializeField]
    private RpcTarget target = RpcTarget.All;

    [PunRPC]
    private void PrivateRpc()
    {
        print("PrivateRpc");
    }

    [PunRPC]
    protected void ProtectedRpc()
    {
        print("ProtectedRpc");
    }

    [PunRPC]
    internal void InternalRpc()
    {
        print("InternalRpc");
    }

    [PunRPC]
    protected internal void ProtectedInternalRpc()
    {
        print("ProtectedInternalRpc");
    }

    [PunRPC]
    public void PublicRpc()
    {
        print("PublicRpc");
    }

    [PunRPC]
    private static void PrivateStaticRpc()
    {
        print("PrivateStaticRpc");
    }

    [PunRPC]
    protected static void ProtectedStaticRpc()
    {
        print("ProtectedStaticRpc");
    }

    [PunRPC]
    internal static void InternalStaticRpc()
    {
        print("InternalStaticRpc");
    }

    [PunRPC]
    protected internal static void ProtectedInternalStaticRpc()
    {
        print("ProtectedInternalStaticRpc");
    }

    [PunRPC]
    public static void PublicStaticRpc()
    {
        print("PublicStaticRpc");
    }

    [PunRPC]
    public void PublicRpcWithOneParameter(object p)
    {
        print("PublicRpcWithOneParameter");
    }

    [PunRPC]
    public void PublicRpcWithTwoParameters(object p1, object p2)
    {
        print("PublicRpcWithTwoParameters");
    }

    [PunRPC]
    public object PublicRpcWithReturn()
    {
        print("PublicRpcWithReturn");
        return null;
    }


    [PunRPC]
    private bool RpcReturnsBool()
    {
        return false;
    }

    [PunRPC]
    private int RpcReturnsInt()
    {
        return 0;
    }

    public override void BaseMethodWithPunRpcAttribute()
    {
        base.BaseMethodWithPunRpcAttribute();
    }

    [PunRPC]
    public override void BaseMethodWithoutPunRpcAttribute()
    {
        base.BaseMethodWithoutPunRpcAttribute();
    }

    
    [PunRPC]
    public void DuplicateRpc()
    {
        print("DuplicateRpc");
    }

    [PunRPC]
    public void DuplicateRpc(PhotonMessageInfo info)
    {
        Debug.LogFormat("DuplicateRpc Sender:{0} PhotonView:{1} PhotonView.Owner:{2} Timestamp:{3}|{4}", 
            info.Sender, info.photonView, info.photonView.Owner, info.SentServerTimestamp, info.SentServerTime);
    }

    [PunRPC]
    private void SameNameDifferentSignature(int i)
    {
        Debug.LogFormat("SameNameDifferentSignature({0})", i);
    }

    [PunRPC]
    private void SameNameDifferentSignature(bool b)
    {
        Debug.LogFormat("SameNameDifferentSignature({0})", b);
    }

    [PunRPC]
    private void SameNameDifferentSignature(string s, object o)
    {
        Debug.LogFormat("SameNameDifferentSignature({0}, {1})", s, o);
    }

    [PunRPC]
    private void SameNameDifferentSignature(int a, int b)
    {
        Debug.LogFormat("SameNameDifferentSignature({0}, {1})", a, b);
    }

    [PunRPC]
    private void OptionalParams(int a = 0, int b = 0)
    {
        Debug.LogFormat("OptionalParams({0}, {1})", a, b);
    }

    [PunRPC]
    private void ObjectArray(object[] objectArray)
    {
        Debug.LogFormat("ObjectArray length={0}", objectArray.Length);
    }

    [PunRPC]
    private void StringArray(string[] stringArray)
    {
        Debug.LogFormat("StringArray length={0}", stringArray.Length);
    }

    [ContextMenu("CallRpc")]
    private void CallRpc()
    {
        if (paramsNumber > 0)
        {
            parameters = new object[paramsNumber];
            for (int i = 0; i < paramsNumber; i++)
            {
                parameters[i] = Random.Range(int.MinValue, int.MaxValue);
            }
        }
        Debug.LogFormat("calling \"{0}\" with {1} parameters, target={2}", methodName, paramsNumber, target);
        if (arrayParam)
        {
            this.photonView.RPC(methodName, target, parameters as object);
        }
        else
        {
            this.photonView.RPC(methodName, target, parameters);
        }
    }
}
