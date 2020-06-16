using System;
using Project.Scripts.StageSelectScene;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using Project.Scripts.Utils.PlayerPrefsUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(Button))]
    public class TreeController : MonoBehaviour
    {
        /// <summary>
        /// 木のレベル
        /// </summary>
        [SerializeField] private ELevelName _levelName;

        /// <summary>
        /// 木のId
        /// </summary>
        [SerializeField] private ETreeId _treeId;

        /// <summary>
        /// 木が解放されているかどうか
        /// </summary>
        [SerializeField] public bool released;

        /// <summary>
        /// 木をクリアしているかどうか
        /// </summary>
        [NonSerialized] public bool cleared = false;

        public void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.TREE_RELEASED + _treeId.ToString());
            if (_treeId != ETreeId.Spring_1)
                PlayerPrefs.DeleteKey(PlayerPrefsKeys.TREE_CLEARED + _treeId.ToString());
        }

        /// <summary>
        /// 木の状態の更新
        /// </summary>
        public void UpdateReleased()
        {
            released = PlayerPrefs.GetInt(PlayerPrefsKeys.TREE_RELEASED + _treeId.ToString(), Default.TREE_RELEASED) == 1;
            if (_treeId != ETreeId.Spring_1)
                cleared = PlayerPrefs.GetInt(PlayerPrefsKeys.TREE_CLEARED + _treeId.ToString(), Default.TREE_CLEARED) == 1;
        }

        /// <summary>
        /// 木の状態の保存
        /// </summary>
        public void SaveReleased()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.TREE_RELEASED + _treeId.ToString(), Convert.ToInt32(released));
            if (_treeId != ETreeId.Spring_1)
                PlayerPrefs.SetInt(PlayerPrefsKeys.TREE_CLEARED + _treeId.ToString(), Convert.ToInt32(cleared));
        }

        /// <summary>
        /// 木が押されたとき
        /// </summary>
        public void TreeButtonDown()
        {
            StageSelectDirector.levelName = _levelName;
            StageSelectDirector.treeId = _treeId;
            TreeLibrary.LoadStageSelectScene(_levelName);
        }
    }
}
