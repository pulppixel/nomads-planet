// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnJoinedInstantiate.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  This component will instantiate a network GameObject when a room is joined
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


namespace Photon.Pun.Tests
{
    using UnityEngine;
    using System.Collections.Generic;

    using Pun;
    using Realtime;

    /// <summary>
    /// This component will instantiate a network GameObject when a room is joined
    /// </summary>
	public class OnJoinedInstantiatePooled : MonoBehaviour, IMatchmakingCallbacks
    {
        public Transform SpawnPosition;
        public float PositionOffset = 0.0f;
        public GameObject[] PrefabsToInstantiate; // set in inspector
        public int InstantiateMultiplier = 10;
        public int Interval = 10;

        private int current = 0;

        public bool OnlyMasterInstantiates = true;

        public bool ApplyNameBasedPool = true;

        private NameBasedPool namebasedPool;

        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            if (this.ApplyNameBasedPool)
            {
                if (this.namebasedPool == null)
                {
                    this.namebasedPool = new NameBasedPool();
                    foreach (GameObject o in this.PrefabsToInstantiate)
                    {
                        this.namebasedPool.SetPrefab(o);
                    }
                }

                PhotonNetwork.PrefabPool = this.namebasedPool;
            }
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        public void OnCreatedRoom()
        {
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinedRoom()
        {
            if (this.OnlyMasterInstantiates && !PhotonNetwork.IsMasterClient)
            {
                return;
            }

            this.InvokeRepeating("Instantiate", 0.5f, this.Interval);
        }

        public void Instantiate()
        {
            if (this.PrefabsToInstantiate != null)
            {
                for (int i = 0; i < this.InstantiateMultiplier; i++)
                {
                    foreach (GameObject o in this.PrefabsToInstantiate)
                    {
                        string name = o.name;
                        //Debug.Log("Instantiating: " + o.name);

                        Vector3 spawnPos = Vector3.up;
                        if (this.SpawnPosition != null)
                        {
                            spawnPos = this.SpawnPosition.position;
                        }

                        int row = PhotonNetwork.LocalPlayer.ActorNumber - 1;

                        if (name.EndsWith("Scn"))
                        {
                            Vector3 itemposScene = spawnPos + new Vector3(row, 0, this.current);
                            this.current = this.current + 1;
                            PhotonNetwork.InstantiateSceneObject(name, itemposScene, Quaternion.identity);
                        }
                        else
                        {
                            Vector3 itempos = spawnPos + new Vector3(row, 0, this.current);
                            this.current = this.current + 1;
                            PhotonNetwork.Instantiate(name, itempos, Quaternion.identity);
                        }
                    }
                }
            }
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public void OnLeftRoom()
        {
            this.CancelInvoke("Instantiate");
        }
    }




    /// <summary>
    /// A "fake" pool
    /// </summary>
    public class NameBasedPool : IPunPrefabPool
    {
        public Dictionary<string, Queue<GameObject>> Pool = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();


        public void SetPrefab(GameObject go)
        {
            string prefabId = go.name;

            //Debug.LogWarning("Pool.Instantiate loads resource " + prefabId);
            bool resActive = go.activeSelf;
            go.SetActive(false);

            this.prefabs[prefabId] = (GameObject)GameObject.Instantiate(go);
            this.prefabs[prefabId].name = prefabId;

            go.SetActive(resActive);

        }

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            Queue<GameObject> list = null;
            this.Pool.TryGetValue(prefabId, out list);

            if (list == null || list.Count == 0)
            {
                //Debug.LogWarning("Pool.Instantiate new " + prefabId);
                if (!this.prefabs.ContainsKey(prefabId))
                {
                    //Debug.LogWarning("Pool.Instantiate loads resource " + prefabId);
                    GameObject res = (GameObject)Resources.Load(prefabId, typeof(GameObject));

                    bool resActive = res.activeSelf;
                    res.SetActive(false);

                    this.prefabs[prefabId] = (GameObject)GameObject.Instantiate(res);
                    this.prefabs[prefabId].name = prefabId;

                    res.SetActive(resActive);
                    Resources.UnloadAsset(res);
                }

                // important: The pool needs to instantiate DISABLED objects! PUN will enable it.

                GameObject go = (GameObject)GameObject.Instantiate(this.prefabs[prefabId], position, rotation);
                go.name = prefabId;

                return go;
            }
            else
            {
                //Debug.Log("Pool.Instantiate pooled " + prefabId);
                GameObject go = list.Dequeue();

                go.transform.position = position;
                go.transform.rotation = rotation;
                this.EnableInstance(go);

                return go;
            }
        }

        public void Destroy(GameObject gameObject)
        {
            //Debug.Log("Pool.Destroy " + gameObject.name, gameObject);

            string prefabId = gameObject.name;  // TODO: Objects could use a better prefab-IDing
            this.DisableInstance(gameObject);

            Queue<GameObject> list = null;
            if (!this.Pool.TryGetValue(prefabId, out list))
            {
                list = new Queue<GameObject>();
                this.Pool[prefabId] = list;
            }

            list.Enqueue(gameObject);
        }


        private void EnableInstance(GameObject gameObject)
        {
            //gameObject.SetActive(true);  // done by PhotonNetwork.Instantiate(), AFTER calling this.
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.WakeUp();
            }
        }

        private void DisableInstance(GameObject gameObject)
        {
            //gameObject.SetActive(false);  // done by PhotonNetwork.Destroy(), right before calling this.
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.Sleep();
            }
        }
    }

}