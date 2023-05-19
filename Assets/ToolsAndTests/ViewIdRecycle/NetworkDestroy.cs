using System.Collections;
using Photon.Pun;
using UnityEngine;


public class NetworkDestroy : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField]
    private float delay;

    [SerializeField]
    private bool destroyOnInstantiate;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        this.name = string.Format("ctp-{0}", this.photonView.ViewID);
        if (this.destroyOnInstantiate)
        {
            this.NetworkDestroyPrefab();
        }
    }

    [ContextMenu("NetworkDestroyPrefab")]
    private void NetworkDestroyPrefab()
    {
        if (this.delay > 0f)
        {
            StartCoroutine(this.NetworkDestroyPrefabWithDelay());
        }
        else
        {
            PhotonNetwork.Destroy(this.photonView);
        }
    }

    private IEnumerator NetworkDestroyPrefabWithDelay()
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(this.photonView);
    }
}
