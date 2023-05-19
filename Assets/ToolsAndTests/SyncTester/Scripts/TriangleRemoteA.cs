using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TriangleRemoteA: GrapherComponent, IOnEventCallback
{
    private float sentTime;

    private Vector3 localObjectStoredPosition;
    private Vector3 localObjectStoredDirection;

    private Vector3 remoteObjectDirection;
    private Vector3 remoteObjectTargetPosition;

    private float remoteObjectDistance;

	//public float LerpWeight = 0.5f;
	//public float LerpFactor = 0.8f;

    public bool UseExtrapolation;

//	[Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered vector and 90 percent of the previously filtered value.")]
//	public float filteringFactor =0.1f;	

	Vector3 filteredVector;

	float fraction;


    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

	
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);

    }

    public void Update()
    {

        UpdateRemoteObject();


    }

	public void LateUpdate()
	{
		if (this.GrapherInterface != null)
		{
			this.GrapherInterface.OnSetRemotePosition (this.transform.position,"A Distance",Color.green);
		}
	}


	float currentPacketTime,timeToReachGoal,currentTime,lastPacketTime;
	Vector3 realPosition,positionAtLastPacket;

    private void UpdateRemoteObject()
    {
		timeToReachGoal = currentPacketTime - lastPacketTime;
		currentTime += Time.deltaTime;
		transform.position = Vector3.Lerp(positionAtLastPacket, realPosition, (float)(currentTime / timeToReachGoal));

    }

    public void OnEvent(EventData photonEvent)
    {
		if (photonEvent.Code == NetworkManager.SEND_UPDATE)
        {
            object[] data = (object[])photonEvent.CustomData;

			this.remoteObjectTargetPosition = (Vector3)data[(int)TriangleLocal.DataIndex.Position];
			this.transform.rotation = (Quaternion)data[(int)TriangleLocal.DataIndex.Rotation];
			this.remoteObjectDirection = (Vector3)data[(int)TriangleLocal.DataIndex.Direction];

			this.filteredVector = this.remoteObjectTargetPosition;

            // Extrapolation
            if (this.UseExtrapolation)
            {
				this.filteredVector += this.remoteObjectDirection;
            }


			this.remoteObjectDistance = Vector3.Distance(this.transform.position, this.filteredVector);

			this.currentTime = 0.0f;
			this.positionAtLastPacket = this.transform.position;
			this.realPosition = this.filteredVector;
			this.lastPacketTime = this.currentPacketTime;
			this.currentPacketTime = (float)data[(int)TriangleLocal.DataIndex.TimeStamp];
        }
    }
}


