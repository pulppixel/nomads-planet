using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TriangleRemoteB : GrapherComponent, IOnEventCallback
{


    private float m_Distance;
    private float m_Angle;

    private Vector3 m_NetworkPosition;
    private Vector3 m_StoredSpeed;
    private float m_StoredTime;

    Vector3 m_Speed;
    Vector3 m_Acc;

    private Vector3 m_NetworkSpeed;
    private Vector3 m_NetworkAcc;

	float fraction;

   // public float AccelerationWeight = 0f;

	public float LerpWeight = 0;

	[Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered vector and 90 percent of the previously filtered value.")]
	public float filteringFactor =0.1f;	

	Vector3 filteredVector;

    public void Awake()
    {
        m_NetworkPosition = Vector3.zero;
    }

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
			this.GrapherInterface.OnSetRemotePosition (this.transform.position,"B Distance",Color.cyan);
		}
	}



    private void UpdateRemoteObject()
    {
		filteredVector.x = ( this.m_NetworkPosition.x * filteringFactor) + (filteredVector.x * (1.0f - filteringFactor));
		filteredVector.y = ( this.m_NetworkPosition.y * filteringFactor) + (filteredVector.y * (1.0f - filteringFactor));
		filteredVector.z = ( this.m_NetworkPosition.z * filteringFactor) + (filteredVector.z * (1.0f - filteringFactor));

		Vector3 _weightedSpeed = this.m_NetworkSpeed;// ((1f - AccelerationWeight) * this.m_NetworkSpeed + AccelerationWeight * (this.m_NetworkAcc * Time.deltaTime));

        Vector3 _target = this.transform.position + _weightedSpeed * Time.deltaTime;

		this.fraction = this.fraction + Time.deltaTime * (NetworkManager.Instance.SendRate-1);
		Vector3 _lerp = Vector3.Lerp(this.transform.position, filteredVector, this.fraction);
		this.transform.position = (1f - LerpWeight) * _target + LerpWeight * _lerp;

	
    }

    public void OnEvent(EventData photonEvent)
    {
		if (photonEvent.Code == NetworkManager.SEND_UPDATE)
        {
            object[] data = (object[])photonEvent.CustomData;

			this.m_NetworkPosition = (Vector3)data[(int)TriangleLocal.DataIndex.Position];
			this.m_NetworkSpeed = (Vector3)data[(int)TriangleLocal.DataIndex.Speed];
			this.m_NetworkAcc = (Vector3)data[(int)TriangleLocal.DataIndex.Acceleration];

			this.transform.rotation = (Quaternion)data[(int)TriangleLocal.DataIndex.Rotation];



        }
    }
}