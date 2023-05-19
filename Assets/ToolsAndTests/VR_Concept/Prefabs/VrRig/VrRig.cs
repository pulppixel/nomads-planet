using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrRig : MonoBehaviour {


    public Transform Head;
    public Transform RightHand;
    public Transform LeftHand;

    public float Speed = 3f;

	// Use this for initialization
	void Start () {
		
	}


    private void Update()
    {
        this.transform.Translate(0f, 0f, Input.GetAxis("Vertical")*Speed*Time.deltaTime,Space.Self);
        this.transform.Rotate(0f, Input.GetAxis("Horizontal")*90*Time.deltaTime, 0f);

        float _leftHandInput = (Input.GetKey(KeyCode.R) ? 1f : 0f) + (Input.GetKey(KeyCode.F) ? -1f : 0f);
        LeftHand.Translate(0f, _leftHandInput * Time.deltaTime, 0f, Space.Self);

        float _rightHandInput = (Input.GetKey(KeyCode.T) ? 1f : 0f)   + (Input.GetKey(KeyCode.G)? -1f: 0f);
        RightHand.Translate(0f, _rightHandInput*Time.deltaTime, 0f, Space.Self);

    }
}
