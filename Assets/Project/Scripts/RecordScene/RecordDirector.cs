using System;
using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using SnapScroll;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.RecordScene
{
    public class RecordDirector : MonoBehaviour
    {
        [SerializeField] private GameObject _graphPrefab;

        [SerializeField] private GameObject _stageNumPrefab;

        [SerializeField] private GameObject _successLinePrefab;

        private readonly Dictionary<EStageLevel, GameObject> levelText = new Dictionary<EStageLevel, GameObject>();

        private readonly Dictionary<EStageLevel, GameObject> percentageText = new Dictionary<EStageLevel, GameObject>();

        private readonly Dictionary<EStageLevel, GameObject> graphArea = new Dictionary<EStageLevel, GameObject>();

        private SnapScrollView snapScrollView;

        private void Awake()
        {
            // 取得
            snapScrollView = GameObject.Find("SnapScrollView").GetComponent<SnapScrollView>();
            // ページの最大値を設定
            snapScrollView.MaxPage = Enum.GetNames(typeof(EStageLevel)).Length - 1;
            // ページの横幅の設定
            snapScrollView.PageSize = Screen.width;

            // TODO: 将来的には非同期で呼び出したい (バージョンアップ待ち)
            // 各種グラフなどを全て描画する
            Draw();
        }

        /* 全難易度の画面を描画する */
        private void Draw()
        {
            foreach (EStageLevel stageLevel in Enum.GetValues(typeof(EStageLevel))) {
                // 準備
                GetGameObjects(stageLevel);
                // 成功割合の描画
                DrawPercentage(stageLevel);
                // 棒グラフの描画
                DrawGraph(stageLevel);
                // タイトルの変更
                levelText[stageLevel].GetComponent<Text>().text = StageInfo.LevelName[stageLevel];
            }
        }

        /* 必要な GameObject を Scene から取得 */
        private void GetGameObjects(EStageLevel stageLevel)
        {
            // SnapScrollView -> Viewport -> Content -> のオブジェクトを特定
            var level = GameObject.Find(stageLevel.ToString());

            // 各種 GameObject を取得
            levelText.Add(stageLevel, level.transform.Find("Level").gameObject);
            percentageText.Add(stageLevel, level.transform.Find("Percentage").gameObject);
            graphArea.Add(stageLevel, level.transform.Find("GraphArea").gameObject);
        }

        /* 難易度に合わせた成功割合を描画する */
        private void DrawPercentage(EStageLevel stageLevel)
        {
            var stageNum = StageInfo.Num[stageLevel];
            var stageStartId = StageInfo.StageStartId[stageLevel];

            var successStageNum = 0;

            for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                var stageStatus = StageStatus.Get(stageId);

                if (stageStatus.passed) {
                    successStageNum++;
                }
            }

            var successPercentage = (successStageNum / (float) stageNum) * 100;

            percentageText[stageLevel].GetComponent<Text>().text = successPercentage + "%";
        }

        /* 難易度に合わせた棒グラフを描画する */
        private void DrawGraph(EStageLevel stageLevel)
        {
            var stageNum = StageInfo.Num[stageLevel];
            var stageStartId = StageInfo.StageStartId[stageLevel];

            // 描画するパネル
            var graphAreaContent = graphArea[stageLevel].GetComponent<RectTransform>();

            /* UIの配置周りの定数宣言 */

            // 棒グラフ描画範囲の左端
            const float leftPosition = 0.1f;

            // 棒グラフ描画範囲の右端
            const float rightPosition = 0.95f;

            // 棒グラフ本体の上端
            const float topPosition = 0.9f;

            // 棒グラフ本体の下端
            const float bottomPosition = 0.15f;

            // 棒グラフの横幅が隙間の何倍か
            const float graphWidthRatio = 1.0f;

            // ステージ番号の下端
            const float bottomStageNumPosition = 0.05f;

            // グラフ間の隙間の横幅 -> stageNum個のグラフと(stageNum + 1)個の隙間
            var blankWidth = (rightPosition - leftPosition) / (stageNum * graphWidthRatio + (stageNum + 1));

            // 棒グラフの横幅
            var graphWidth = blankWidth * graphWidthRatio;

            // 挑戦回数の最大値を求める
            var maxChallengeNum = GetMaxChallengeNum(stageNum, stageStartId);

            // 目盛の最大値を求める
            var maxScale = (float) Math.Ceiling((float) maxChallengeNum / 30) * 30;

            // 目盛を書き換える
            if (maxScale > 0) {
                graphArea[stageLevel].transform.Find("Scale4-Value").gameObject.GetComponent<Text>().text = maxScale.ToString();
                graphArea[stageLevel].transform.Find("Scale3-Value").gameObject.GetComponent<Text>().text = (maxScale * 2 / 3).ToString();
                graphArea[stageLevel].transform.Find("Scale2-Value").gameObject.GetComponent<Text>().text = (maxScale / 3).ToString();
            }

            // ステージ番号
            var stageName = 1;
            // 描画する棒グラフの左端を示す
            var left = leftPosition;

            for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                // 左端と右端の更新
                left += blankWidth;
                var right = left + graphWidth;

                var stageStatus = StageStatus.Get(stageId);

                /* ステージ番号の配置 */
                var stageNumUi = Instantiate(_stageNumPrefab);
                stageNumUi.transform.SetParent(graphAreaContent, false);
                stageNumUi.GetComponent<Text>().text = stageName.ToString();
                stageNumUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, bottomStageNumPosition);
                stageNumUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, bottomPosition);

                /* 棒グラフの配置 */
                var graphUi = Instantiate(_graphPrefab);
                graphUi.transform.SetParent(graphAreaContent, false);

                // 挑戦回数に応じた棒グラフの上端
                var graphMaxY = bottomPosition;

                if (maxChallengeNum != 0) {
                    // 挑戦回数を用いて，棒グラフの高さを描画範囲に正規化する
                    graphMaxY = (topPosition - bottomPosition) * (stageStatus.challengeNum / maxScale) + bottomPosition;
                }

                graphUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, bottomPosition);
                graphUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, graphMaxY);

                if (stageStatus.passed) {
                    /* 成功している場合は，色を水色にして，成功した際の挑戦回数も示す */
                    graphUi.GetComponent<Image>().color = Color.cyan;

                    var successLineUi = Instantiate(_successLinePrefab);
                    successLineUi.transform.SetParent(graphAreaContent, false);
                    var successY = (topPosition - bottomPosition) * (stageStatus.firstSuccessNum / maxScale) +
                        bottomPosition;
                    successLineUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, successY);
                    successLineUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, successY);
                } else {
                    /* 成功していない場合は，色を赤色にする */
                    graphUi.GetComponent<Image>().color = Color.red;
                }

                // 左端の更新
                left = right;
                // ステージ番号の更新
                stageName++;
            }
        }

        /* 挑戦回数の最大値を求める */
        private static int GetMaxChallengeNum(int stageNum, int stageStartId)
        {
            var maxChallengeNum = 0;

            for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                var stageStatus = StageStatus.Get(stageId);

                if (stageStatus.challengeNum > maxChallengeNum) {
                    maxChallengeNum = stageStatus.challengeNum;
                }
            }

            return maxChallengeNum;
        }
    }
}
