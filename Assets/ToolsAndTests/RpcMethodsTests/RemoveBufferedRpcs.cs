using Photon.Pun;
using UnityEngine;

public class RemoveBufferedRpcs : MonoBehaviour
{
    [SerializeField]
    private int photonViewId;
    [SerializeField]
    private string rpcMethodName;
    [SerializeField]
    private int[] callerActorNumbers;
    //[SerializeField]
    //private int[] rpcMethodParameters;

    [ContextMenu("RemoveBufferedRpcs")]
    private void RemoveBufferedRpcsMethod()
    {
        //object[] parameters = null;
        //if (this.rpcMethodParameters != null && this.rpcMethodParameters.Length > 0)
        //{
        //    parameters = new object[this.rpcMethodParameters.Length];
        //    for (int i = 0; i < parameters.Length; i++)
        //    {
        //        parameters[i] = this.rpcMethodParameters[i];
        //    }
        //}
        PhotonNetwork.RemoveBufferedRPCs(this.photonViewId, this.rpcMethodName, this.callerActorNumbers/*, parameters*/);
    }
}
