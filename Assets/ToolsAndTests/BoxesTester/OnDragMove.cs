using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;


public class OnDragMove : MonoBehaviourPun, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool following;
	public bool OnlyIfIsMine = true;

    public float factor = 0.1f;
    private Vector3 target;
    private Collider coll;

    public void OnBeginDrag(PointerEventData eventData)
    {
		if (OnlyIfIsMine && !this.photonView.IsMine)
		{
			return;
		}

        this.following = true;
        this.coll = GameObject.Find("Ground").GetComponent<Collider>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (this.following)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (this.coll.Raycast(ray, out hit, 100.0F))
            {
                this.target = hit.point + new Vector3(0, 0.5f);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.following = false;
    }

    public void Update()
    {
        if (this.following)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.target, this.factor);
        }
    }
}