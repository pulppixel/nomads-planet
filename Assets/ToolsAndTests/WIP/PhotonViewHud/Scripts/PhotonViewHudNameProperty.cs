using UnityEngine.UI;

using Photon.Pun.Demo.Cockpit;

public class PhotonViewHudNameProperty : PropertyListenerBase {

	public Text Text;

	string _cache = null;

	PhotonViewHud _hud;

	void Awake()
	{
		_hud = this.GetComponentInParent<PhotonViewHud> ();
	}

	void Update()
	{
		if (_hud.Target != null) {
			
			if (_cache ==null || _hud.Target.name != _cache) {
				_cache = _hud.Target.name;
				Text.text = _cache;
				this.OnValueChanged ();
			}
		} else {
			if (_cache != null)
			{
				_cache = null;
				Text.text = "n/a";
			}
		}

	}
}