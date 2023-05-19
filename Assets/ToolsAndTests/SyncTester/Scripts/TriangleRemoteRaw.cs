using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TriangleRemoteRaw : GrapherComponent, IOnEventCallback
{

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

	public void LateUpdate()
	{
		if (GrapherInterface != null)
		{
			GrapherInterface.OnSetRemotePosition(this.transform.position , "Raw",Color.white);
		}
	}


    public void OnEvent(EventData photonEvent)
    {
		if (photonEvent.Code == NetworkManager.SEND_UPDATE)
        {

            object[] data = (object[])photonEvent.CustomData;

			Vector3 _pos = (Vector3)data [(int)TriangleLocal.DataIndex.Position];
			this.transform.position = _pos;
			this.transform.rotation = (Quaternion)data[(int)TriangleLocal.DataIndex.Rotation];


		}


    }

}