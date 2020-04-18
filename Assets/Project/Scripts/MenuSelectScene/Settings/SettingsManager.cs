using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        public static float BGMVolume
        {
            get => PlayerPrefs.GetFloat(PlayerPrefsKeys.BGM_VOLUME, Default.BGM_VOLUME);
            set => PlayerPrefs.SetFloat(PlayerPrefsKeys.BGM_VOLUME, value);
        }

        public static float SEVolume
        {
            get => PlayerPrefs.GetFloat(PlayerPrefsKeys.SE_VOLUME, Default.SE_VOLUME);
            set => PlayerPrefs.SetFloat(PlayerPrefsKeys.SE_VOLUME, value);
        }

        public static int StageDetails
        {
            get => PlayerPrefs.GetInt(PlayerPrefsKeys.STAGE_DETAILS, Default.STAGE_DETAILS);
            set => PlayerPrefs.SetInt(PlayerPrefsKeys.STAGE_DETAILS, value);
        }
    }
}
