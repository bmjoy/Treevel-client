using System;
using Project.Scripts.StageSelectScene;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Library;
using Project.Scripts.Utils.PlayerPrefsUtils;
using Project.Scripts.GameDatas;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.LevelSelect
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class TreeController : MonoBehaviour
    {
        /// <summary>
        /// 現在の木の状態
        /// </summary>
        [SerializeField] public ETreeState state;

        /// <summary>
        /// クリア状態を判定するクラス
        /// </summary>
        [SerializeField] private IClearTreeHandler _clearHandler;

        /// <summary>
        /// 木のレベル
        /// </summary>
        [SerializeField] private ELevelName _levelName;

        /// <summary>
        /// 木のId
        /// </summary>
        [SerializeField] public ETreeId treeId;

        [SerializeField] private Material _material;

        public void Awake()
        {
            // クリア条件を実装するクラスを指定する
            _clearHandler = TreeInfo.CLEAR_HANDLER[treeId];
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeys.TREE + treeId.ToString());
        }

        /// <summary>
        /// 木の状態の更新
        /// </summary>
        public void UpdateReleased()
        {
            // 現在状態をDBから得る
            state = (ETreeState) Enum.ToObject(typeof(ETreeState), PlayerPrefs.GetInt(PlayerPrefsKeys.TREE + treeId.ToString(), Default.TREE_STATE));
            // 状態の更新
            switch (state) {
                case ETreeState.Unreleased: {
                        // 何もしない
                        GetComponent<Image>().material = _material;
                        break;
                    }
                case ETreeState.Released: {
                        // Implementorに任せる
                        GetComponent<Image>().material = null;
                        state = _clearHandler.IsClear(treeId);
                        break;
                    }
                case ETreeState.Cleared: {
                        // 全クリアかどうかをチェックする
                        GetComponent<Image>().material = null;
                        var stageNum = TreeInfo.NUM[treeId];
                        var allStageCleared = true;
                        for (var stageNumber = 1; stageNumber <= stageNum; stageNumber++) {
                            allStageCleared = allStageCleared && StageStatus.Get(treeId, stageNum).cleared;
                        }
                        if (allStageCleared) {
                            state = ETreeState.Finished;
                        }
                        break;
                    }
                case ETreeState.Finished: {
                        // アニメーション
                        GetComponent<Image>().material = null;
                        Debug.Log($"{treeId} is finished.");
                        break;
                    }
                default: {
                        throw new NotImplementedException();
                    }
            }
        }

        /// <summary>
        /// 木の状態の保存
        /// </summary>
        public void SaveReleased()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.TREE + treeId.ToString(), Convert.ToInt32(state));
        }

        /// <summary>
        /// 木が押されたとき
        /// </summary>
        public void TreeButtonDown()
        {
            StageSelectDirector.levelName = _levelName;
            StageSelectDirector.treeId = treeId;
            TreeLibrary.LoadStageSelectScene(_levelName);
        }
    }
}
