using System;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.TextUtils;
using UnityEngine;

namespace Project.Scripts.Utils.PlayerPrefsUtils
{
    public static class UserSettings
    {
        private static ELanguage _currentLanguage;

        public static ELanguage CurrentLanguage
        {
            get => _currentLanguage;
            set {
                _currentLanguage = value;
                LanguageUtility.DoOnLanguageChange();

                // PlayerPrefsに保存
                MyPlayerPrefs.SetObject(PlayerPrefsKeys.LANGUAGE, _currentLanguage);
            }
        }

        private static float _BGMVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM_VOLUME, Default.BGM_VOLUME);

        public static float BGMVolume
        {
            get => _BGMVolume;
            set {
                _BGMVolume = value;
                PlayerPrefs.SetFloat(PlayerPrefsKeys.BGM_VOLUME, _BGMVolume);
            }
        }

        private static float _SEVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SE_VOLUME, Default.SE_VOLUME);

        public static float SEVolume
        {
            get => _SEVolume;
            set {
                _SEVolume = value;
                PlayerPrefs.SetFloat(PlayerPrefsKeys.SE_VOLUME, _SEVolume);
            }
        }

        private static int _stageDetails = PlayerPrefs.GetInt(PlayerPrefsKeys.STAGE_DETAILS, Default.STAGE_DETAILS);

        public static int StageDetails
        {
            get => _stageDetails;
            set {
                _stageDetails = value;
                PlayerPrefs.SetInt(PlayerPrefsKeys.STAGE_DETAILS, _stageDetails);
            }
        }

        static UserSettings()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKeys.LANGUAGE)) {
                _currentLanguage = MyPlayerPrefs.GetObject<ELanguage>(PlayerPrefsKeys.LANGUAGE);
            } else {
                var systemLanguage = Application.systemLanguage.ToString();
                if (!Enum.TryParse(systemLanguage, out _currentLanguage)) {
                    _currentLanguage = Default.LANGUAGE;
                }
            }
        }
    }
}
