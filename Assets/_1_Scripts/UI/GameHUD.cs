using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private LoadingFaderController faderController;
        [SerializeField] private ResultBoard resultBoard;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(.25f);
            StartCoroutine(faderController.FadeOut());
        }

        public void OpenResultBoard()
        {
            SoundManager.Instance.PlayChangeBgm();
            resultBoard.gameObject.SetActive(true);
            resultBoard.Entrance();
        }

        public void LeaveGame()
        {
            StartCoroutine(LeaveLogic());
        }

        private IEnumerator LeaveLogic()
        {
#if !UNITY_SERVER
            VivoxVoiceManager.Instance.DisconnectAllChannels();
            VivoxVoiceManager.Instance.Logout();
#endif
            yield return StartCoroutine(faderController.FadeIn());
            yield return new WaitForSeconds(.2f);

            if (NetworkManager.Singleton.IsHost)
            {
                HostSingleton.Instance.GameManager.Shutdown();
            }

            ClientSingleton.Instance.GameManager.Disconnect();
        }
    }
}