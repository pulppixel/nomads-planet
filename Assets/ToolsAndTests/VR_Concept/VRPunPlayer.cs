using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// VR Pun player.
/// 
/// In Multi player VR environment, the local player is usually invisible because it's a first person view, while remotly, it needs to be represented with an avatar.
/// 
/// This Networked Component leverage both case with a single Prefab for the player,
/// and instantiate/handle manually the VR rig and the Avatar to prevent otherwise complex setup the Networked prefabs hould handle both internally
/// 
/// 
/// This Component catches up with the VR rig position and rotation so that PhotonTransformView can be used, otherwise smoothing would have to be implemented manually.
/// It also perfectly ok to have this GameObject not moving at all and simply write the VR rig movement in the PhotoStream (using IPunObservable interface) and the remote instance smooth out the value manually.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class VRPunPlayer : MonoBehaviourPunCallbacks, IPunObservable
{

    /// <summary>
    /// The local vr rig prefab to instantiate when this instante is the local player.
    /// </summary>
    public GameObject LocalVrRigPrefab;

    /// <summary>
    /// The remote avatar prefab to instantiate when this instance is the remote player.
    /// </summary>
    public GameObject RemoteAvatarPrefab;

    /// <summary>
    /// The local Player vr rig. Remotly, it will be represented with the RemoteAvatar GameObject
    /// </summary>
    VrRig LocalVrRig;

    /// <summary>
    /// The remote avatar which represent this Player. Locally, it will be handled by the LocalVrRig
    /// </summary>
    VrAvatar RemoteAvatar;

    // The next variables are the network positions and rotations to match. 
    // They are cached because OnPhotonSerializeView can be skipped if no data has to be read,
    // so all the work for smoothing is done inside a regular update call
    Vector3 RemoteAvatarHeadTarget;
    Quaternion RemoteAvatarHeadRotation;

    Vector3 RemoteAvatarRightHandTarget;
    Quaternion RemoteAvatarRightHandRotation;

    Vector3 RemoteAvatarLeftHandTarget;
    Quaternion RemoteAvatarLeftHandRotation;

    float LerpSpeed = 5f;

    void Start()
    {

        if ( this.photonView.IsMine)
        {
            LocalVrRig = Instantiate(LocalVrRigPrefab).GetComponent<VrRig>();
        }
        else
        {
            RemoteAvatar = Instantiate(RemoteAvatarPrefab).GetComponent<VrAvatar>();
        }
    }

    void OnDestroy()
    {
        if (RemoteAvatar != null)
        {
            Destroy(RemoteAvatar.gameObject);
        }

        if (LocalVrRig != null)
        {
            Destroy(LocalVrRig.gameObject);
        }
    }

    void Update()
    {
        if (!this.photonView.IsMine && RemoteAvatar != null)
        {
            RemoteAvatar.transform.position = this.transform.position;
            RemoteAvatar.transform.rotation = this.transform.rotation;

            RemoteAvatar.HeadIKEffector.localPosition = Vector3.Lerp(RemoteAvatar.HeadIKEffector.localPosition, RemoteAvatarHeadTarget, LerpSpeed * Time.deltaTime);
            RemoteAvatar.HeadIKEffector.localRotation = Quaternion.Slerp(RemoteAvatar.HeadIKEffector.localRotation, RemoteAvatarHeadRotation, LerpSpeed * Time.deltaTime);

            RemoteAvatar.RightHandIkEffector.localPosition = Vector3.Lerp(RemoteAvatar.RightHandIkEffector.localPosition, RemoteAvatarRightHandTarget, LerpSpeed * Time.deltaTime);
            RemoteAvatar.RightHandIkEffector.localRotation = Quaternion.Slerp(RemoteAvatar.RightHandIkEffector.localRotation, RemoteAvatarRightHandRotation, LerpSpeed * Time.deltaTime);

            RemoteAvatar.LeftHandIkEffector.localPosition = Vector3.Lerp(RemoteAvatar.LeftHandIkEffector.localPosition, RemoteAvatarLeftHandTarget, LerpSpeed * Time.deltaTime);
            RemoteAvatar.LeftHandIkEffector.localRotation = Quaternion.Slerp(RemoteAvatar.LeftHandIkEffector.localRotation, RemoteAvatarLeftHandRotation, LerpSpeed * Time.deltaTime);

        }
    }

    void LateUpdate()
    {
        if (this.photonView.IsMine && LocalVrRig!=null)
        {
            this.transform.position = LocalVrRig.transform.position;
            this.transform.rotation = LocalVrRig.transform.rotation;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            if (LocalVrRig !=null)
            {
                stream.SendNext(LocalVrRig.Head.transform.localPosition);
                stream.SendNext(LocalVrRig.Head.transform.localRotation);

                stream.SendNext(LocalVrRig.RightHand.transform.localPosition);
                stream.SendNext(LocalVrRig.RightHand.transform.localRotation);

                stream.SendNext(LocalVrRig.LeftHand.transform.localPosition);
                stream.SendNext(LocalVrRig.LeftHand.transform.localRotation);
            }
        }
        else
        {
            if (RemoteAvatar != null)
            {
                // store variables and work with them during a regular update to avoid stall if OnPhotonSerializeView is not called.
                RemoteAvatarHeadTarget = (Vector3)stream.ReceiveNext();
                RemoteAvatarHeadRotation = (Quaternion)stream.ReceiveNext();

                RemoteAvatarRightHandTarget = (Vector3)stream.ReceiveNext();
                RemoteAvatarRightHandRotation = (Quaternion)stream.ReceiveNext();

                RemoteAvatarLeftHandTarget = (Vector3)stream.ReceiveNext();
                RemoteAvatarLeftHandRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
