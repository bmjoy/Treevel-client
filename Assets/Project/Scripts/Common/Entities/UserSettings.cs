using System;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Common.Entities
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
                PlayerPrefsUtility.SetObject(Constants.PlayerPrefsKeys.LANGUAGE, _currentLanguage);
            }
        }

        public static IReadOnlyReactiveProperty<float> BGMVolume => _BGMVolume;

        private static readonly FloatReactiveProperty _BGMVolume =
            new FloatReactiveProperty(PlayerPrefs.GetFloat(Constants.PlayerPrefsKeys.BGM_VOLUME, Default.BGM_VOLUME));

        public static void SetBGMVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0.0f, 1.0f);
            if (volume.Equals(_BGMVolume.Value))
                return;

            _BGMVolume.Value = volume;
            SoundManager.Instance.ResetVolume();
        }

        public static IReadOnlyReactiveProperty<float> SEVolume => _SEVolume;

        private static readonly FloatReactiveProperty _SEVolume =
            new FloatReactiveProperty(PlayerPrefs.GetFloat(Constants.PlayerPrefsKeys.SE_VOLUME, Default.SE_VOLUME));

        public static void SetSEVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0.0f, 1.0f);
            if (volume.Equals(_BGMVolume.Value))
                return;

            _SEVolume.Value = volume;
            SoundManager.Instance.ResetVolume();
        }

        private static int _stageDetails = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.STAGE_DETAILS, Default.STAGE_DETAILS);

        public static int StageDetails
        {
            get => _stageDetails;
            set {
                _stageDetails = value;
                PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.STAGE_DETAILS, _stageDetails);
            }
        }

        private static float _levelSelectCanvasScale = PlayerPrefs.GetFloat(Constants.PlayerPrefsKeys.LEVEL_SELECT_CANVAS_SCALE, Default.LEVEL_SELECT_CANVAS_SCALE);

        public static float LevelSelectCanvasScale
        {
            get => _levelSelectCanvasScale;
            set {
                _levelSelectCanvasScale = value;
                PlayerPrefs.SetFloat(Constants.PlayerPrefsKeys.LEVEL_SELECT_CANVAS_SCALE, _levelSelectCanvasScale);
            }
        }

        private static Vector2 _levelSelectScrollPosition;

        public static Vector2 LevelSelectScrollPosition
        {
            get => _levelSelectScrollPosition;
            set {
                _levelSelectScrollPosition = value;
                PlayerPrefsUtility.SetObject(Constants.PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION, _levelSelectScrollPosition);
            }
        }

        static UserSettings()
        {
            if (PlayerPrefs.HasKey(Constants.PlayerPrefsKeys.LANGUAGE)) {
                _currentLanguage = PlayerPrefsUtility.GetObject<ELanguage>(Constants.PlayerPrefsKeys.LANGUAGE);
            } else {
                var systemLanguage = Application.systemLanguage.ToString();
                if (!Enum.TryParse(systemLanguage, out _currentLanguage)) {
                    _currentLanguage = Default.LANGUAGE;
                }
            }

            if (PlayerPrefs.HasKey(Constants.PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION)) {
                _levelSelectScrollPosition = PlayerPrefsUtility.GetObject<Vector2>(Constants.PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION);
            } else {
                _levelSelectScrollPosition = Default.LEVEL_SELECT_SCROLL_POSITION;
            }
        }

        public static void ToDefault()
        {
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.BGM_VOLUME);
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.SE_VOLUME);
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.STAGE_DETAILS);

            SetBGMVolume(Default.BGM_VOLUME);
            SetSEVolume(Default.SE_VOLUME);
            StageDetails = Default.STAGE_DETAILS;
        }
    }
}
