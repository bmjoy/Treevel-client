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
        public static void LoadStageSelectScene(ELevelName levelName)
        {
            switch (levelName) {
                case ELevelName.Easy:
                    AddressableAssetManager.LoadScene(SceneName.SPRING_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.Normal:
                    AddressableAssetManager.LoadScene(SceneName.SUMMER_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.Hard:
                    AddressableAssetManager.LoadScene(SceneName.AUTOMN_STAGE_SELECT_SCENE);
                    break;
                case ELevelName.VeryHard:
                    AddressableAssetManager.LoadScene(SceneName.WINTER_STAGE_SELECT_SCENE);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
