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

        private static float _canvasScale = PlayerPrefs.GetFloat(PlayerPrefsKeys.CANVAS_SCALE, Default.CANVAS_SCALE);

        public static float CanvasScale
        {
            get => _canvasScale;
            set {
                _canvasScale = value;
                PlayerPrefs.SetFloat(PlayerPrefsKeys.CANVAS_SCALE, _canvasScale);
            }
        }

        private static Vector2 _scrollPosition;

        public static Vector2 ScrollPosition
        {
            get => _scrollPosition;
            set {
                _scrollPosition = value;
                MyPlayerPrefs.SetObject(PlayerPrefsKeys.SCROLL_POSITION, _scrollPosition);
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

            if (PlayerPrefs.HasKey(PlayerPrefsKeys.SCROLL_POSITION)) {
                _scrollPosition = MyPlayerPrefs.GetObject<Vector2>(PlayerPrefsKeys.SCROLL_POSITION);
            } else {
                _scrollPosition = Default.SCROLL_POSITION;
            }
        }
    }
}
