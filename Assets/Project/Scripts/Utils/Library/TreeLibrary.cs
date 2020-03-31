using System;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.Utils.Library
{
    public static class TreeLibrary
    {
        /// <summary>
        /// レベルに応じたStageSelectSceneをロードする
        /// </summary>
        /// <param name="sceneName"> 木のレベル </param>
        public static void LoadStageSelectScene(ELevelName levelName) {
            switch (levelName) {
                case ELevelName.Easy:
                    AddressableAssetManager.Instance.LoadScene(SceneName.SPRING_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.Normal:
                    AddressableAssetManager.Instance.LoadScene(SceneName.SUMMER_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.Hard:
                    AddressableAssetManager.Instance.LoadScene(SceneName.AUTOMN_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.VeryHard:
                    AddressableAssetManager.Instance.LoadScene(SceneName.WINTER_STAGE_SELECT_SCENE);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
