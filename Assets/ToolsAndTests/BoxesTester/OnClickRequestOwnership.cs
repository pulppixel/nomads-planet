using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(PhotonView))]
public class OnClickRequestOwnership : MonoBehaviourPun, IPointerClickHandler
{
    public void OnClick()
    {
        OnPointerClick(null);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Vector3 colVector = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            this.photonView.RPC("ColorRpc", RpcTarget.AllBufferedViaServer, colVector);
        }
        else
        {
            if (eventData == null || eventData.button == PointerEventData.InputButton.Left)
            {
                if (this.photonView.ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    Debug.Log("Not requesting ownership. Already mine.");
                    return;
                }
            

                this.photonView.RequestOwnership();
            }
        }
    }

    [PunRPC]
    public void ColorRpc(Vector3 col)
    {
        Color color = new Color(col.x, col.y, col.z);
        this.gameObject.GetComponent<Renderer>().material.color = color;
    }
    
}