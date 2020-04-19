using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public static class SettingsManager
    {
        private static float _BGMVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM_VOLUME, Default.BGM_VOLUME);

        public static float BGMVolume
        {
            get => _BGMVolume;
            set
            {
                _BGMVolume = value;
                PlayerPrefs.SetFloat(PlayerPrefsKeys.BGM_VOLUME, _BGMVolume);
            }
        }

        private static float _SEVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SE_VOLUME, Default.SE_VOLUME);

        public static float SEVolume
        {
            get => _SEVolume;
            set
            {
                _SEVolume = value;
                PlayerPrefs.SetFloat(PlayerPrefsKeys.SE_VOLUME, _SEVolume);
            }
        }

        private static int _stageDetails = PlayerPrefs.GetInt(PlayerPrefsKeys.STAGE_DETAILS, Default.STAGE_DETAILS);

        public static int StageDetails
        {
            get => _stageDetails;
            set
            {
                _stageDetails = value;
                PlayerPrefs.SetInt(PlayerPrefsKeys.STAGE_DETAILS, _stageDetails);
            }
        }
    }
}
