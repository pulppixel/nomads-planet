using UnityEngine;
using System.Collections;
using Photon.Pun;

public class PhotonTransformView3 : MonoBehaviourPun, IPunObservable
{
	public Vector3 realPosition = Vector3.zero;
	public Vector3 positionAtLastPacket = Vector3.zero;
	public double currentTime = 0.0;
	public double currentPacketTime = 0.0;
	public double lastPacketTime = 0.0;
	public double timeToReachGoal = 0.0;

	private Quaternion m_NetworkRotation;

	void Update ()
	{
		if (!photonView.IsMine)
		{
			timeToReachGoal = currentPacketTime - lastPacketTime;
			currentTime += Time.deltaTime;
			transform.position = Vector3.Lerp(positionAtLastPacket, realPosition, (float)(currentTime / timeToReachGoal));

			transform.rotation = m_NetworkRotation;
		}
	}


	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else
		{
			currentTime = 0.0;
			positionAtLastPacket = transform.position;
			realPosition = (Vector3)stream.ReceiveNext();
			lastPacketTime = currentPacketTime;
			currentPacketTime = info.SentServerTime;    // TODO: check if this needs SentServerTime or SentServerTimestamp

			this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
		}
	}
}