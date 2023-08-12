using TMPro;
using UnityEngine;

namespace NomadsPlanet.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField joinCodeField;
        
        public async void StartHost()
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }

        public async void StartClient()
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
        }
    }
}