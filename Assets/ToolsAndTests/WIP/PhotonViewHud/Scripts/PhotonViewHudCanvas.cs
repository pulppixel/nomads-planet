using UnityEngine;

using Photon.Pun;

using System.Collections.Generic;

public class PhotonViewHudCanvas : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks {

	public PhotonViewHud HudPrefab;

	public static PhotonViewHudCanvas Instance;

	public List<PhotonViewHudProxy> HudList = new List<PhotonViewHudProxy>();

	public Color[] Colors;

	float _currentColorSelection;

	void Awake()
	{
		Colors [0] = Color.red;
		var colorhsv = new ColorHSV(Colors[0]);
		for (var i = 1; i < Colors.Length; i++) {
			colorhsv.h += 300f / Colors.Length; // don't go to 360 otherwise last color is too close to first color
			Colors[i] = colorhsv.ToColor();
		}


		Instance = this;


	}

	//void Start()
	//{
	//	PhotonNetwork.AddCallbackTarget (this); // this class is a MonoBehaviourPunCallbacks, which adds callback targets in OnEnable!
	//}


	public PhotonViewHud MountHud(PhotonViewHudProxy Target)
	{
		GameObject _go = Instantiate (HudPrefab.gameObject);

		PhotonViewHud _hud = _go.GetComponent<PhotonViewHud> ();

		HudList.Add (Target);

		_go.transform.SetParent (this.transform);

		_hud.SetTarget (Target);

		return _hud;
	}

	public void UnMountHud(PhotonViewHudProxy hud)
	{
		HudList.Remove(hud);
	}
		
	public void UnMountHud(PhotonView photonView)
	{
		HudList.RemoveAll( p => p.photonView == photonView );
	}
		
	public void OnColorizePhotonViewsToggleChanged(int selection)
	{
		_currentColorSelection = selection;


		foreach (PhotonViewHudProxy _hud in HudList) {
			if (_hud != null ) {
				_hud.RefreshColor ();
			}
		}
	}


	public Color GetColor(PhotonViewHudProxy proxy)
	{
		if (_currentColorSelection == 1) {
			
			if (proxy.photonView.OwnerActorNr == -1) {
				return Color.black;
			}

			return  Colors[proxy.photonView.OwnerActorNr];
		}

		if (_currentColorSelection == 2) {
			
			if (proxy.photonView.ControllerActorNr == -1) {
				return Color.black;
			}
			return  Colors [proxy.photonView.ControllerActorNr];
		}

		if (_currentColorSelection == 3) {
			
			if (proxy.photonView.CreatorActorNr == -1) {
				return Color.black;
			}
			return  Colors [proxy.photonView.CreatorActorNr];
		}

		return Color.white;
	}

	#region IPunOwnershipCallbacks implementation
	public void OnOwnershipRequest (PhotonView targetView, Photon.Realtime.Player requestingPlayer)
	{
		Debug.Log ("OnOwnershipRequest" + targetView +" "+requestingPlayer);

	}

	public void OnOwnershipTransfered (PhotonView targetView, Photon.Realtime.Player previousOwner)
	{
		Debug.Log ("OnOwnershipTransfered" + targetView +" "+previousOwner);

		// brute force, maybe we should use a bottom up approach coming for individual change watch
		foreach (PhotonViewHudProxy _hud in HudList) {
			if (_hud != null ) {
				_hud.RefreshColor ();
			}
		}
	}

    /// <inheritdoc />
    public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
    {
        // implement if you need to react to failed ownership transfer requests
    }

    #endregion
}
