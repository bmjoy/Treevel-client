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
                LanguageUtility.OnLanguageChange?.Invoke();

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
                SoundManager.Instance.ResetVolume();
            }
        }

        private static float _SEVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SE_VOLUME, Default.SE_VOLUME);

        public static float SEVolume
        {
            get => _SEVolume;
            set {
                _SEVolume = value;
                PlayerPrefs.SetFloat(PlayerPrefsKeys.SE_VOLUME, _SEVolume);
                SoundManager.Instance.ResetVolume();
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

        private static float _levelSelectCanvasScale = PlayerPrefs.GetFloat(PlayerPrefsKeys.LEVEL_SELECT_CANVAS_SCALE, Default.LEVEL_SELECT_CANVAS_SCALE);

        public static float LevelSelectCanvasScale
        {
            get => _levelSelectCanvasScale;
            set {
                _levelSelectCanvasScale = value;
                PlayerPrefs.SetFloat(PlayerPrefsKeys.LEVEL_SELECT_CANVAS_SCALE, _levelSelectCanvasScale);
            }
        }

        private static Vector2 _levelSelectScrollPosition;

        public static Vector2 LevelSelectScrollPosition
        {
            get => _levelSelectScrollPosition;
            set {
                _levelSelectScrollPosition = value;
                MyPlayerPrefs.SetObject(PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION, _levelSelectScrollPosition);
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

            if (PlayerPrefs.HasKey(PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION)) {
                _levelSelectScrollPosition = MyPlayerPrefs.GetObject<Vector2>(PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION);
            } else {
                _levelSelectScrollPosition = Default.LEVEL_SELECT_SCROLL_POSITION;
            }
        }

        public static void ToDefault()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.BGM_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.SE_VOLUME);
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.STAGE_DETAILS);

            BGMVolume = Default.BGM_VOLUME;
            SEVolume = Default.SE_VOLUME;
            StageDetails = Default.STAGE_DETAILS;
        }
    }
}
