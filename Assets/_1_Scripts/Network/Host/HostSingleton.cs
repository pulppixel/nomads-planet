using System.Threading.Tasks;
using UnityEngine;

namespace NomadsPlanet
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;

        private HostGameManager _gameManager;

        public static HostSingleton Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            instance = this;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CreateHost()
        {
            _gameManager = new HostGameManager();
        }
    }
}