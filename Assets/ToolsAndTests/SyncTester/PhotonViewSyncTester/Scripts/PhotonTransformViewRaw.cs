// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms without any smoothing or alteration via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace Photon.Pun
{
	[AddComponentMenu("Photon Networking/Photon Transform View Raw")] 
	[RequireComponent(typeof(PhotonView))]
	public class PhotonTransformViewRaw : MonoBehaviour, IPunObservable
	{
		public bool m_SynchronizePosition = true;
		public bool m_SynchronizeRotation = true;
		public bool m_SynchronizeScale = false;
	
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				if (this.m_SynchronizePosition)
				{
					stream.SendNext(transform.position);
				}

				if (this.m_SynchronizeRotation)
				{
					stream.SendNext(transform.rotation);
				}

				if (this.m_SynchronizeScale)
				{
					stream.SendNext(transform.localScale);
				}
			}
			else
			{
				if (this.m_SynchronizePosition)
				{
					transform.position = (Vector3)stream.ReceiveNext();

				}

				if (this.m_SynchronizeRotation)
				{
					this.transform.localRotation = (Quaternion)stream.ReceiveNext ();
				}

				if (this.m_SynchronizeScale)
				{
					transform.localScale = (Vector3)stream.ReceiveNext();
				}
			}
		}
	}
}