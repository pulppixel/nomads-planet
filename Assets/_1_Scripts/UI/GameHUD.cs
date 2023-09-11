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
#if !UNITY_SERVER
            VivoxVoiceManager.Instance.Logout();
#endif
            StartCoroutine(LeaveLogic());
        }

        private IEnumerator LeaveLogic()
        {
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