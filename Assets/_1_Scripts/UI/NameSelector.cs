using System.Collections;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NomadsPlanet.Utils;
using UnityEngine.Playables;

namespace NomadsPlanet
{
    public class NameSelector : MonoBehaviour
    {
        [SerializeField] private AddressableManager addressableManager;
        
        [SerializeField] private LoadingFaderController loadingFader;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private Button connectButton;
        [SerializeField] private int minNameLength = 1;
        [SerializeField] private int maxNameLength = 12;

        [SerializeField] private PlayableDirector playableDirector;

        private void Start()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                SceneManager.LoadScene(SceneName.NetBootStrap);
                return;
            }

            nameField.text = ES3.LoadString(PrefsKey.NameKey, string.Empty);
            HandleNameChanged();
        }

        public void HandleNameChanged()
        {
            nameField.text = Regex.Replace(
                nameField.text,
                "[a-zA-Z]", match => match.Value.ToUpperInvariant()
            );

            connectButton.interactable =
                nameField.text.Length > minNameLength &&
                nameField.text.Length <= maxNameLength;
        }

        public void SetTime(float time)
        {
            playableDirector.Stop();
            playableDirector.time = time;
            playableDirector.Play();
        }

        public void RestartTimeline()
        {
            playableDirector.Stop();
            playableDirector.time = 0f;
            playableDirector.Play();
        }

        public void Connect()
        {
            connectButton.interactable = false;
            StartCoroutine(ConnectLogic());
        }

        private IEnumerator ConnectLogic()
        {
            yield return StartCoroutine(addressableManager.DownloadAllAssets());
            yield return StartCoroutine(loadingFader.FadeIn());
            yield return new WaitForSeconds(.2f);

            ES3.Save(PrefsKey.NameKey, nameField.text);
            SceneManager.LoadScene(SceneName.NetBootStrap);
        }
    }
}