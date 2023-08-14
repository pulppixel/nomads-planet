using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    public class NameSelector : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private Button connectButton;
        [SerializeField] private int minNameLength = 1;
        [SerializeField] private int maxNameLength = 12;

        private void Start()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                SceneManager.LoadScene(SceneName.NetBootStrap);
                return;
            }

            nameField.text = ES3.LoadString(PrefsKey.PlayerNameKey, string.Empty);
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

        public void Connect()
        {
            ES3.Save(PrefsKey.PlayerNameKey, nameField.text);
            SceneManager.LoadScene(SceneName.NetBootStrap);
        }
    }
}