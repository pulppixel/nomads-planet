using UnityEngine;
using UnityEngine.UI;

using Photon.Pun.Demo.Cockpit;

public class PhotonViewHudIsSceneViewProperty : PropertyListenerBase {

	public Text Text;

	public Color TrueColor;
	public Color FalseColor;
	public Color DefaultColor;

	int _cache = -1;

	PhotonViewHud _hud;

	void Awake()
	{
		_hud = this.GetComponentInParent<PhotonViewHud> ();
	}

	void Update()
	{
		if (_hud.Target != null) {
			
			if (_hud.Target.photonView.IsSceneView && _cache!=1 || !_hud.Target.photonView.IsSceneView && _cache!=0 ) {
				_cache = _hud.Target.photonView.IsSceneView ? 1 : 0;
				Text.text =  _hud.Target.photonView.IsSceneView?"TRUE":"FALSE";
				Text.color = _hud.Target.photonView.IsSceneView?TrueColor:FalseColor;

				this.OnValueChanged ();
			}
		} else {
			if (_cache != -1)
			{
				_cache = -1;
				Text.color = DefaultColor;
				Text.text = "n/a";
			}
		}

	}
}