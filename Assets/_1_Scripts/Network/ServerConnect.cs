using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class ServerConnect : MonoBehaviour
    {
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        
        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}