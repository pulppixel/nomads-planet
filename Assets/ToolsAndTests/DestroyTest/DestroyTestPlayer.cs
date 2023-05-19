using UnityEngine;
using System.Collections;
using Photon.Pun;

public class DestroyTestPlayer : MonoBehaviourPunCallbacks
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnDisable()
	{
		Debug.Log ("On Disable : photoView : " + this.photonView +" Ower : "+this.photonView.Owner);
	}

    void OnDestroy()
    {
		Debug.Log ("On Destroy : photoView : " + this.photonView +" Owner : "+this.photonView.Owner);
    }


}
