using System;
using Treevel.Common.Managers;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Common.Entities
{
    public class UserSettings
    {
        public static UserSettings Instance = new UserSettings();

        public readonly ReactiveProperty<ELanguage> CurrentLanguage = new ReactiveProperty<ELanguage>(ELanguage.Japanese);

        public readonly ReactiveProperty<float> BGMVolume =
            new ReactiveProperty<float>(PlayerPrefs.GetFloat(Constants.PlayerPrefsKeys.BGM_VOLUME, Default.BGM_VOLUME));

        public readonly ReactiveProperty<float> SEVolume =
            new ReactiveProperty<float>(PlayerPrefs.GetFloat(Constants.PlayerPrefsKeys.SE_VOLUME, Default.SE_VOLUME));

        private int _stageDetails = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.STAGE_DETAILS, Default.STAGE_DETAILS);

        public int StageDetails {
            get => Instance._stageDetails;
            set {
                Instance._stageDetails = value;
                PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.STAGE_DETAILS, Instance._stageDetails);
            }
        }

        private float _levelSelectCanvasScale =
            PlayerPrefs.GetFloat(Constants.PlayerPrefsKeys.LEVEL_SELECT_CANVAS_SCALE,
                                 Default.LEVEL_SELECT_CANVAS_SCALE);

        public float LevelSelectCanvasScale {
            get => Instance._levelSelectCanvasScale;
            set {
                Instance._levelSelectCanvasScale = value;
                PlayerPrefs.SetFloat(Constants.PlayerPrefsKeys.LEVEL_SELECT_CANVAS_SCALE, Instance._levelSelectCanvasScale);
            }
        }

        private Vector2 _levelSelectScrollPosition;

        public Vector2 LevelSelectScrollPosition {
            get => Instance._levelSelectScrollPosition;
            set {
                Instance._levelSelectScrollPosition = value;
                PlayerPrefsUtility.SetObject(Constants.PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION,
                                             Instance._levelSelectScrollPosition);
            }
        }

        private UserSettings()
        {
            if (PlayerPrefs.HasKey(Constants.PlayerPrefsKeys.LANGUAGE)) {
                CurrentLanguage.Value = PlayerPrefsUtility.GetObject<ELanguage>(Constants.PlayerPrefsKeys.LANGUAGE);
            } else {
                var systemLanguage = Application.systemLanguage.ToString();
                if (!Enum.TryParse(systemLanguage, out ELanguage initLanguage)) {
                    initLanguage = Default.LANGUAGE;
                }

                CurrentLanguage.Value = initLanguage;
            }

            CurrentLanguage.Subscribe(language => PlayerPrefsUtility.SetObject(Constants.PlayerPrefsKeys.LANGUAGE, language));
            BGMVolume.Subscribe(value => {
                PlayerPrefs.SetFloat(Constants.PlayerPrefsKeys.BGM_VOLUME, value);
                SoundManager.Instance.ResetVolume();
            });
            SEVolume.Subscribe(value => {
                PlayerPrefs.SetFloat(Constants.PlayerPrefsKeys.SE_VOLUME, value);
                SoundManager.Instance.ResetVolume();
            });

            if (PlayerPrefs.HasKey(Constants.PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION)) {
                _levelSelectScrollPosition =
                    PlayerPrefsUtility.GetObject<Vector2>(Constants.PlayerPrefsKeys.LEVEL_SELECT_SCROLL_POSITION);
            } else {
                _levelSelectScrollPosition = Default.LEVEL_SELECT_SCROLL_POSITION;
            }
        }

        public void ToDefault()
        {
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.BGM_VOLUME);
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.SE_VOLUME);
            PlayerPrefs.DeleteKey(Constants.PlayerPrefsKeys.STAGE_DETAILS);

            SEVolume.Value = Default.SE_VOLUME;
            BGMVolume.Value = Default.BGM_VOLUME;
            StageDetails = Default.STAGE_DETAILS;
        }
    }
}
