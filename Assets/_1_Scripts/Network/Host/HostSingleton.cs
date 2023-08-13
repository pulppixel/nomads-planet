using System;
using System.Threading.Tasks;
using UnityEngine;

namespace NomadsPlanet
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;

        public HostGameManager GameManager { get; private set; }

        public static HostSingleton Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = FindObjectOfType<HostSingleton>();

                if (instance != null)
                {
                    return instance;
                }

                Debug.LogError("HostSingleton이 씬에 없습니다!");
                return null;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CreateHost()
        {
            GameManager = new HostGameManager();
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}