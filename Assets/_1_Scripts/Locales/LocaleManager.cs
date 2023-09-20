using System;
using NomadsPlanet.Utils;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace NomadsPlanet
{
    public class LocaleManager : MonoBehaviour
    {
        private string _defaultLocale = "en";
        [SerializeField] private GameObject enAudio;
        [SerializeField] private GameObject koAudio;

        private void Start()
        {
            _defaultLocale = ES3.LoadString(PrefsKey.LocaleKey, "en");
            UpdateLocale(_defaultLocale);
        }

        public void UpdateLocale(string languageIdentifier)
        {
            enAudio.SetActive(_defaultLocale == "en");
            koAudio.SetActive(_defaultLocale == "ko");
            ES3.Save(PrefsKey.LocaleKey, languageIdentifier);
            LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
                LocaleIdentifier anIdentifier = aLocale.Identifier;
                if (anIdentifier == localeCode)
                {
                    LocalizationSettings.SelectedLocale = aLocale;
                }
            }
        }
    }
}