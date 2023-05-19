using UnityEngine.UI;

using Photon.Pun.Demo.Cockpit;

public class PhotonViewHudCreatorActorNrProperty : PropertyListenerBase {

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
			
			if (_hud.Target.photonView.CreatorActorNr != _cache) {
				_cache = _hud.Target.photonView.CreatorActorNr;
				Text.text = ""+ _cache;
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