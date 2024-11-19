using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private Button _turkishButton;
    [SerializeField] private Button _englishButton;
    [SerializeField] private Button _spanishButton;

    
    private void Awake() 
    {
        _turkishButton.onClick.AddListener(() => SetLanguage(LanguageType.Turkish));
        _englishButton.onClick.AddListener(() => SetLanguage(LanguageType.English));
        _spanishButton.onClick.AddListener(() => SetLanguage(LanguageType.Spanish));  
    }

    public void SetLanguage(LanguageType languageType)
    {
        foreach(var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if(locale.LocaleName.Equals(languageType.ToString()))
            {
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log("Language set to: " + locale.LocaleName);
                return;
            }
        }

        Debug.LogWarning("Locale not found for " + languageType.ToString());
    }
}
