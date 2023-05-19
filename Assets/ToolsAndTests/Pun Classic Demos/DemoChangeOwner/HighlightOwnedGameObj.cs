using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof (PhotonView))]
public class HighlightOwnedGameObj : MonoBehaviourPun
{
    public GameObject PointerPrefab;
    public float Offset = 0.5f;
    private Transform markerTransform;
    
    private int assignedColorForUserId = -1;
    Renderer m_Renderer;

    public bool ShowAll = true;

    public void Start()
    {
        GameObject markerObject = (GameObject) GameObject.Instantiate(this.PointerPrefab);
        markerObject.transform.parent = gameObject.transform;
        this.markerTransform = markerObject.transform;
        
        m_Renderer = this.markerTransform.gameObject.GetComponent<Renderer>();

        this.markerTransform.gameObject.SetActive(false);
    }


    // Update is called once per frame
    private void Update()
    {
        if (this.ShowAll || photonView.IsMine)
        {
            this.markerTransform.gameObject.SetActive(true);

            if (assignedColorForUserId != this.photonView.ControllerActorNr)
            {
                this.assignedColorForUserId = this.photonView.ControllerActorNr;
                m_Renderer.material.color = MaterialPerOwner.Colors[this.assignedColorForUserId];
            }

            Vector3 parentPos = gameObject.transform.position;
            this.markerTransform.position = new Vector3(parentPos.x, parentPos.y + this.Offset, parentPos.z);
            this.markerTransform.rotation = Quaternion.identity;
        }
        else 
        {
            this.markerTransform.gameObject.SetActive(false);
        }
    }
}