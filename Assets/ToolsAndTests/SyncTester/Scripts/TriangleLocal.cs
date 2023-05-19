using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;


public class TriangleLocal : GrapherComponent
{

	public enum DataIndex {Position=0,Rotation=1,Direction=2,Speed=3,Acceleration=4,TimeStamp=5};

    private float sentTime;

    private Vector3 localObjectStoredPosition;
    private Vector3 localObjectStoredDirection;

    private Vector3 remoteObjectDirection;
    private Vector3 remoteObjectTargetPosition;

    private float remoteObjectDistance;

	private Vector3 m_StoredSpeed;
	private float m_StoredTime;
	Vector3 m_Speed;
	Vector3 m_Acc;

    private RaiseEventOptions raiseEventOptions;

    private SendOptions sendOptions;

	public bool watchTransformOnLateUpdate;

	public bool forceUpMove;

    [Range(1.0f, 10.0f)]
    public float MovementSpeed = 5.0f;

	public GameObject RemoteObjectPrefab;

	GameObject RemoteObject;


	public int SkipNthEvent = 0;

	int _skipCounter;

    public void Awake()
    {

        raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        };

        sendOptions = new SendOptions
        {
            Reliability = true
        };
    }


    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);

		float repeat = 1.0f / NetworkManager.Instance.SendRate;

        InvokeRepeating("SendUpdate", 0.0f, repeat);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);

        CancelInvoke();
    }

    public void Update()
    {

        UpdateLocalObject();

    }

	public void LateUpdate()
	{
		
		if (watchTransformOnLateUpdate)
		{
			
			this.m_Speed = (transform.position - this.localObjectStoredPosition) / (Time.realtimeSinceStartup - this.m_StoredTime);


			localObjectStoredDirection = transform.position - localObjectStoredPosition;
			localObjectStoredPosition = transform.position;

			this.m_Acc = (this.m_Speed - m_StoredSpeed) / (Time.realtimeSinceStartup - this.m_StoredTime);

			if (GrapherInterface != null)
			{
				GrapherInterface.OnSetValue (this.m_Acc.x, "acc X", Color.red);
				GrapherInterface.OnSetValue (this.m_Acc.y, "acc y", Color.green);

				GrapherInterface.OnSetValue (this.m_Speed.y, "Speed y", Color.cyan);
			}

			this.m_StoredTime = Time.realtimeSinceStartup;
			this.m_StoredSpeed = this.m_Speed;
		}

		if (GrapherInterface != null)
		{
			//	Debug.Log ("OnSetLocalPosition");
			//	GrapherInterface.OnSetTrigger("lateUpdate",Color.grey);
			GrapherInterface.OnSetLocalPosition(transform.position);
			//	GrapherInterface.OnSetValue (this.m_Acc.x, "acc X", Color.red);
			//	GrapherInterface.OnSetValue (this.m_Acc.y, "acc y", Color.green);
		}


	}


    private void SendUpdate()
    {


		if (!watchTransformOnLateUpdate)
		{
			this.m_Speed = (transform.position - this.localObjectStoredPosition) / (Time.realtimeSinceStartup - this.m_StoredTime);


			localObjectStoredDirection = transform.position - localObjectStoredPosition;
			localObjectStoredPosition = transform.position;

			this.m_Acc = (this.m_Speed - m_StoredSpeed) / (Time.realtimeSinceStartup - this.m_StoredTime);

			if (GrapherInterface != null)
			{
				GrapherInterface.OnSetValue (this.m_Acc.x, "acc X", Color.red);
				GrapherInterface.OnSetValue (this.m_Acc.y, "acc y", Color.green);

				GrapherInterface.OnSetValue (this.m_Speed.y, "Speed y", Color.cyan);
			}

			this.m_StoredTime = Time.realtimeSinceStartup;
			this.m_StoredSpeed = this.m_Speed;
		}

		if (_skipCounter >= SkipNthEvent)
		{
			_skipCounter = 0;
			// increase send rate
			// but only force a sending every third
			// and definitly send en an event when you detect an abrupt change
			object[] data = new object[] {
				transform.position,
				transform.rotation,
				localObjectStoredDirection,
				this.m_Speed,
				this.m_Acc,
				(float)PhotonNetwork.Time
			};

			PhotonNetwork.RaiseEvent (NetworkManager.SEND_UPDATE, data, raiseEventOptions, sendOptions);
			PhotonNetwork.SendAllOutgoingCommands ();

			if (GrapherInterface != null)
			{
				GrapherInterface.OnSetValue (1f,"Event",Color.red);
			}
		}
		else
		{
			if (SkipNthEvent>0 && GrapherInterface != null)
			{
				GrapherInterface.OnSetValue (0f,"Event",Color.red);
			}
			_skipCounter++;
		}
	
	
    }

    private void UpdateLocalObject()
    {
        float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		if (forceUpMove)
		{
			v = 1f;
		}

        if (h > 0.2f)
        {
            transform.position += Vector3.right * MovementSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        }
        else if (h < -0.2f)
        {
            transform.position += Vector3.left * MovementSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        }

        if (v > 0.2f)
        {
            transform.position += Vector3.up * MovementSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else if (v < -0.2f)
        {
            transform.position += Vector3.down * MovementSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
        }
    }
}