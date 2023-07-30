using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class JoinServer : MonoBehaviour
    {
        public void Join()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}