using System;
using System.Threading.Tasks;
using UnityEngine;

namespace NomadsPlanet.Client
{
    public class ClientSingleton : MonoBehaviour
    {
        private static ClientSingleton instance;

        private ClientGameManager _gameManager;

        public static ClientSingleton Instance => instance;

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

        public async Task CreateClient()
        {
            _gameManager = new ClientGameManager();

            await _gameManager.InitAsync();
        }
    }
}