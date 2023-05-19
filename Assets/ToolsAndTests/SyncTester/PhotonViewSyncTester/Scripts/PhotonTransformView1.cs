using UnityEngine;

namespace Photon.Pun
{
    [RequireComponent(typeof(PhotonView))]
	public class PhotonTransformView1 : MonoBehaviourPun, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;

        private PhotonView m_PhotonView;

        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;
		private Vector3 m_StoredSpeed;
		private float m_StoredTime;

		Vector3 m_Speed;
		Vector3 m_Acc;


		public float AccelerationWeight = 0.2f; 

		public float LerpWeight = 0.9f;

        public void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            m_StoredPosition = transform.position;
            m_NetworkPosition = Vector3.zero;

        }

        public void Update()
        {
			if (!this.m_PhotonView.IsMine)
            {
				Vector3 _weightedSpeed = ((2f-AccelerationWeight)*this.m_Speed + AccelerationWeight*(this.m_Acc* Time.deltaTime))/2f;

				Vector3 _target = transform.position + _weightedSpeed * Time.deltaTime;

				Vector3 _lerp = Vector3.Lerp(transform.position , this.m_NetworkPosition, 0.4f);
				transform.position = ((2f-LerpWeight)*_target + LerpWeight*_lerp) / 2f;
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
			if (stream.IsWriting)
            {
					this.m_Speed = (transform.position - this.m_StoredPosition) / (Time.realtimeSinceStartup - this.m_StoredTime);

                    this.m_StoredPosition = transform.position;

					this.m_Acc = (this.m_Speed - m_StoredSpeed) / (Time.realtimeSinceStartup - this.m_StoredTime);

					this.m_StoredTime = Time.realtimeSinceStartup;
					this.m_StoredSpeed = this.m_Speed;

                    stream.SendNext(transform.position);
					stream.SendNext(this.m_Speed);
					stream.SendNext(this.m_Acc);
            }
            else
            {                
                    this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
					this.m_Speed = (Vector3)stream.ReceiveNext();
					this.m_Acc = (Vector3)stream.ReceiveNext();
            }
        }
    }
}