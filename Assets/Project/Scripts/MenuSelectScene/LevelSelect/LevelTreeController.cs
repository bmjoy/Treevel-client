using System;
using Project.Scripts.StageSelectScene;
using Project.Scripts.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(Button))]
    public class LevelTreeController : MonoBehaviour
    {
        /// <summary>
        /// 木のレベル
        /// </summary>
        [SerializeField] private ELevelName _levelName;

        /// <summary>
        /// 木のId
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        public void TreeButtonDown()
        {
            StageSelectDirector.levelName = _levelName;
            StageSelectDirector.treeId = _treeId;
            TreeLibrary.LoadStageSelectScene(_levelName);
        }
    }
}
