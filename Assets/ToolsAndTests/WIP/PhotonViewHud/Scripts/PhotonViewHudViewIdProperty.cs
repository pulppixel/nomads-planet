using UnityEngine.UI;

using Photon.Pun.Demo.Cockpit;

public class PhotonViewHudViewIdProperty : PropertyListenerBase {

	public Text Text;

	int _cache = -2;

	PhotonViewHud _hud;

	void Awake()
	{
		_hud = this.GetComponentInParent<PhotonViewHud> ();
	}

	void Update()
	{
		if (_hud.Target != null) {
			
			if (_cache ==-2 || _hud.Target.photonView.ViewID != _cache) {
				_cache = _hud.Target.photonView.ViewID;
				Text.text = _cache.ToString();
				this.OnValueChanged ();
			}
		} else {
			if (_cache != -2)
			{
				_cache = -2;
				Text.text = "n/a";
			}
		}

	}
}