using UnityEngine.UI;

using Photon.Pun.Demo.Cockpit;

public class PhotonViewHudIsMineToggleProperty : PropertyListenerBase {

	public Toggle Toggle;

	bool _cache = false;

	PhotonViewHud _hud;

	void Awake()
	{
		_hud = this.GetComponentInParent<PhotonViewHud> ();
	}

	void Update()
	{
		if (_hud.Target != null) {
			
			if (_hud.Target.photonView.IsMine != _cache) {
				_cache = _hud.Target.photonView.IsMine;
				Toggle.isOn = _cache;

				this.OnValueChanged ();
			}
		} else {
			if (_cache)
			{
				_cache = false;
				Toggle.isOn = _cache;
			}
		}

	}
}