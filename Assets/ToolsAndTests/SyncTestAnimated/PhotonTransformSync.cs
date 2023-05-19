// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformSync.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;


namespace Photon.Pun
{
    using UnityEngine;


    [AddComponentMenu("Photon Networking/Photon TransformSync")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    [RequireComponent(typeof(PhotonView))]
    public class PhotonTransformSync : MonoBehaviour, IPunObservable
    {
        public bool m_SynchronizePosition, m_SynchronizeRotation, m_SynchronizeScale;

        private Vector3 NetPosition;
        private Quaternion NetRotation;
        private Vector3 NetScale;

        public Material RemoteMaterial;

        private struct Snapshot
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
            public int ServerTimeOnSend;
        }

        private List<Snapshot> Snapshots = new List<Snapshot>();


        private float timeOfFirstUpdate = 0.0f;
        private bool gotFirstUpdate;
        public float delay = 0.5f;      // in inspector
        private GameObject remoteObj;

        private float timeInSegment;
        public double PNTime;

        public int currentServerTimestamp;
        public int lastServerTimestamp;



        double lastTime;
        float lastTimeUnity;

        private float normalizedTimeInSegment;
        private Vector3 pos;

        private Transform target;

        public void Awake()
        {
            this.remoteObj = (GameObject)GameObject.CreatePrimitive(PrimitiveType.Quad);
            this.remoteObj.GetComponent<Renderer>().material = this.RemoteMaterial;
            this.remoteObj.transform.position = this.transform.position;

            this.target = this.remoteObj.transform;
        }



        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (this.m_SynchronizePosition)
                {
                    stream.SendNext(this.transform.position);
                }

                if (this.m_SynchronizeRotation)
                {
                    stream.SendNext(this.transform.rotation);
                }

                if (this.m_SynchronizeScale)
                {
                    stream.SendNext(this.transform.localScale);
                }
            }
            else
            {
                this.gotFirstUpdate = true;
                Snapshot snap = new Snapshot { ServerTimeOnSend = info.SentServerTimestamp };

                if (this.m_SynchronizePosition)
                {
                    snap.Position = (Vector3)stream.ReceiveNext();
                }

                if (this.m_SynchronizeRotation)
                {
                    snap.Rotation = (Quaternion)stream.ReceiveNext();
                }

                if (this.m_SynchronizeScale)
                {
                    snap.Scale = (Vector3)stream.ReceiveNext();
                }

                //Debug.Log("Received time: "+ (float)info.timestamp + " now: " + PhotonNetwork.Time + " age: "+ (PhotonNetwork.Time- (float)info.timestamp));
                this.Snapshots.Add(snap);
            }
        }



        public void Update()
        {
            if (this.Snapshots.Count < 4)
            {
                return;
            }


            int delayMilliseconds = (int)(this.delay * 1000);
            int localSimTimestamp = PhotonNetwork.ServerTimestamp - delayMilliseconds;

            while (localSimTimestamp - this.Snapshots[2].ServerTimeOnSend > 0)
            {
                this.Snapshots.RemoveAt(0);
            }
            if (this.Snapshots.Count < 4)
            {
                Debug.LogWarning("Lag compensation buffer not filled: " + (float)PhotonNetwork.Time);
                return;
            }

            int msDeltaOfSegment = this.Snapshots[2].ServerTimeOnSend - this.Snapshots[1].ServerTimeOnSend;
            this.normalizedTimeInSegment = (float)(localSimTimestamp - this.Snapshots[1].ServerTimeOnSend) / (float)msDeltaOfSegment;   // how "far" is the delayed simulation towards the "end" of this segment

            if (this.m_SynchronizePosition)
            {
                this.pos = this.GetCatmullRomPosition(this.normalizedTimeInSegment, this.Snapshots[0].Position, this.Snapshots[1].Position, this.Snapshots[2].Position, this.Snapshots[3].Position);
                this.target.position = this.pos;   // TODO: turn this into a developer option
            }

            if (this.m_SynchronizeRotation)
            {
                this.target.rotation = Quaternion.Slerp(this.Snapshots[1].Rotation, this.Snapshots[2].Rotation, this.normalizedTimeInSegment);
            }

            if (this.m_SynchronizeScale)
            {
                this.target.localScale = Vector3.Lerp(this.Snapshots[1].Scale, this.Snapshots[2].Scale, this.normalizedTimeInSegment);
            }
        }

        
        
        // Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        // http://www.iquilezles.org/www/articles/minispline/minispline.htm
        // https://www.habrador.com/tutorials/interpolation/1-catmull-rom-splines/
        Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }




        public bool GizmoAllSnapshots;
        public bool GizmoPath;
        void OnDrawGizmos()
        {
            if (this.GizmoAllSnapshots)
            {
                Gizmos.color = Color.yellow;
                foreach (Snapshot snap in this.Snapshots)
                {
                    Gizmos.DrawSphere(snap.Position, 0.05f);
                }
            }


            if (this.GizmoPath && this.Snapshots.Count >= 4)
            {
                //The spline's resolution
                //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
                float resolution = 0.2f;

                //How many times should we loop?
                int loops = Mathf.FloorToInt(1f / resolution);
                //The start position of the line
                Vector3 lastPos = this.Snapshots[1].Position;

                for (int i = 1; i <= loops; i++)
                {
                    //Which t position are we at?
                    float t = i * resolution;

                    //Find the coordinate between the end points with a Catmull-Rom spline
                    Vector3 newPos = this.GetCatmullRomPosition(t, this.Snapshots[0].Position, this.Snapshots[1].Position, this.Snapshots[2].Position, this.Snapshots[3].Position);

                    //Draw this line segment
                    Gizmos.color = Color.yellow * t;
                    Gizmos.DrawLine(lastPos, newPos);

                    //Save this pos so we can draw the next line segment
                    lastPos = newPos;
                }
            }
        }

    }
}