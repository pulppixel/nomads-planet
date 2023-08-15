using System.Threading.Tasks;
using UnityEngine;

namespace NomadsPlanet
{
    public class ClientSingleton : MonoBehaviour
    {
        private static ClientSingleton _instance;

        public ClientGameManager GameManager { get; private set; }
        
        public static ClientSingleton Instance
        {
            get
            {
                if (_instance != null) { return _instance; }

                _instance = FindObjectOfType<ClientSingleton>();

                return _instance == null ? null : _instance;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task<bool> CreateClient()
        {
            GameManager = new ClientGameManager();

            return await GameManager.InitAsync();
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}