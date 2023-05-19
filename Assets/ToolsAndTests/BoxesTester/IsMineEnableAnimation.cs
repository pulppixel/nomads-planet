using UnityEngine;
using System.Collections;
using Photon.Pun;


public class IsMineEnableAnimation : MonoBehaviourPun
{
    private Animator myAnim;

    void Awake()
    {
        this.myAnim = this.GetComponent<Animator>();
        Debug.Log("Awake: " + this.myAnim);
    }
    void Start()
    {
        this.myAnim = this.GetComponent<Animator>();
        Debug.Log("Start: " + this.myAnim);
    }

    void OnEnable()
    {
        if (this.myAnim == null)
        {
            Debug.LogError("Is null: myAnim. ",this);
            return;
        }
        if (this.photonView == null)
        {
            Debug.LogError("Is null: photonView", this);
            return;
        }
        this.myAnim.enabled = this.photonView.IsMine;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
