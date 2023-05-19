using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrAvatar : MonoBehaviour {

    public Animator animator;

    public Transform HeadIKEffector;

    public Transform RightHandIkEffector;
    public Transform LeftHandIkEffector;
    public Transform GazeIkEffector;
   
	// Use this for initialization
	void Start () {

     
    }
    

    void OnAnimatorIK(int layer)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetLookAtWeight(1f);

        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIkEffector.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIkEffector.rotation);

        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIkEffector.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, RightHandIkEffector.rotation);

        animator.SetLookAtPosition(GazeIkEffector.position);
    }
}
