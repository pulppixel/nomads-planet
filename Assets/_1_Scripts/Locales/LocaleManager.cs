using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace NomadsPlanet
{
    public class LocaleManager : MonoBehaviour
    {
        public void UpdateLocale(string languageIdentifier)
        {
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