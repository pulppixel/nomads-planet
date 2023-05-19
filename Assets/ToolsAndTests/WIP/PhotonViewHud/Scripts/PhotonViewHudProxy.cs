using UnityEngine;

using Photon.Pun;

public class PhotonViewHudProxy : MonoBehaviourPunCallbacks {

	Material _m;

	// Use this for initialization
	void Start () {
	    if (PhotonViewHudCanvas.Instance == null)
	    {
	        this.enabled = false;
	        return;
	    }

		_m = this.GetComponent<Renderer>().material;

		PhotonViewHudCanvas.Instance.MountHud (this);

		RefreshColor ();
	}



	void OnDestroy ()
	{
	    if (PhotonViewHudCanvas.Instance == null)
	    {
	        return;
	    }
        PhotonViewHudCanvas.Instance.UnMountHud (this);
	}

	public override void OnJoinedRoom ()
	{
		RefreshColor ();
	}

	public override void OnLeftRoom ()
	{
		RefreshColor ();
	}
		

	public void RefreshColor()
	{
		_m.color = PhotonViewHudCanvas.Instance.GetColor (this);
	}


}
