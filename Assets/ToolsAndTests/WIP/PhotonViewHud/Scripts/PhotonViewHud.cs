using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class PhotonViewHud : MonoBehaviour {

	public PhotonViewHudProxy Target;
	public bool Follow = true;

	bool _fullDetails = false;

	public bool FullDetails {
		get {
			return _fullDetails;
		}
		set {
			
			_fullDetails = value;

			if (TopPanel!=null)
			{
				TopPanel.SetActive (_fullDetails);
			}
		}
	}

	public GameObject BottomPanel;
	public GameObject TopPanel;

	Canvas _canvas;

	RectTransform _rt;

	Transform _targetTransform;

	float Offset = 0f;

	CanvasGroup _content;


	// Use this for initialization
	void Start () {

		_canvas = this.GetComponentInParent<Canvas> ();
		_content = this.GetComponentInChildren<CanvasGroup> ();

		_rt = this.GetComponent<RectTransform>();
		if (_rt == null)
		{
			Debug.LogError("We need a RectTransform",this);
			return;
		}

		TopPanel.SetActive (_fullDetails);
	}


	public void SetTarget(PhotonViewHudProxy target)
	{
		Target = target;

		if (Target != null) {
			_targetTransform = Target.transform;
			if (_rt != null) _rt.gameObject.SetActive (true);
		} else {
			if (_rt != null) _rt.gameObject.SetActive (false);
		}

	}

	public void IsDirty()
	{
		Target.RefreshColor ();
	}


	void LateUpdate()
	{

		if (_rt == null || _targetTransform == null) {
			Destroy (this.gameObject);
			return;
		}

		_content.alpha = Target.gameObject.activeInHierarchy ? 1f : 0f;
		_content.interactable = Target.gameObject.activeInHierarchy;
		_content.blocksRaycasts = Target.gameObject.activeInHierarchy;

		if (Follow) {
			//Convert the player's position to the UI space then apply the offset
			transform.position = worldToUISpace (_canvas, _targetTransform.position) + new Vector3 (0f, Offset, 0f);
		}

	}

	Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
	{
		if (Camera.main == null) {
			return Vector3.zero;
		}
		
		//Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
		Vector2 movePos;

		//Convert the screenpoint to ui rectangle local point
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
		//Convert the local point to world point
		return parentCanvas.transform.TransformPoint(movePos);
	}



}
