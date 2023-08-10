using System;
using System.Threading.Tasks;
using NomadsPlanet.Client;
using UnityEngine;

namespace NomadsPlanet
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private HostSingleton hostPrefab;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isDedicatedServer)
        {
            if (isDedicatedServer)
            {
            }
            else
            {
                ClientSingleton clientSingleton = Instantiate(clientPrefab);
                bool authenticated = await clientSingleton.CreateClient();

                HostSingleton hostSingleton = Instantiate(hostPrefab);
                hostPrefab.CreateHost();

                if (authenticated)
                {
                    clientSingleton.GameManager.GoToMenu();
                }
            }
        }
    }
}