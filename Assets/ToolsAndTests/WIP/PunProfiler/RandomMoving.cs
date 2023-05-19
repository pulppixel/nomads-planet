using Photon.Pun;
using UnityEngine;

public class RandomMoving : MonoBehaviourPun {

	public float speed  = 10.0f;
	Vector3 vel ;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (this.photonView.IsMine)
		{
			
			vel = Random.insideUnitSphere * speed;
			transform.Translate (vel * Time.deltaTime);
		}
	}
}
