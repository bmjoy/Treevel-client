using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Common.Utils;
using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.Patterns;
using SnapScroll;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.MenuSelectScene.Record
{
    public class RecordDirector : SingletonObject<RecordDirector>
    {
        [SerializeField] private GameObject _graphPrefab;

        [SerializeField] private GameObject _stageNumPrefab;

        [SerializeField] private GameObject _successLinePrefab;

        private readonly Dictionary<ELevelName, GameObject> _percentageText = new Dictionary<ELevelName, GameObject>();

        private readonly Dictionary<ELevelName, GameObject> _graphArea = new Dictionary<ELevelName, GameObject>();

        private SnapScrollView _snapScrollView;

        /* 以下，グラフ描画 UI 周りの配置設定 */

        // 棒グラフ描画範囲の左端
        private const float _LEFT_POSITION = 0.1f;

        // 棒グラフ描画範囲の右端
        private const float _RIGHT_POSITION = 0.95f;

        // 棒グラフ本体の上端
        private const float _TOP_POSITION = 0.9f;

        // 棒グラフ本体の下端
        private const float _BOTTOM_POSITION = 0.15f;

        // 棒グラフの横幅が隙間の何倍か
        private const float _GRAPH_WIDTH_RATIO = 1.0f;

        // ステージ番号の下端
        private const float _BOTTOM_STAGE_NUM_POSITION = 0.05f;

        // 成功している場合のグラフの色
        [SerializeField] private Color _successGraphColor = Color.cyan;

        // 成功していない場合のグラフの色
        [SerializeField] private Color _notSuccessGraphColor = Color.red;

        private void Awake()
        {
            // 取得
            _snapScrollView = FindObjectOfType<SnapScrollView>();
            // ページの最大値を設定
            _snapScrollView.MaxPage = 2;
            // ページの横幅の設定
            _snapScrollView.PageSize = RuntimeConstants.ScaledCanvasSize.SIZE_DELTA.x;

            // 各種グラフなどを全て描画する
            // Draw();
        }

        public void MoveToRight()
        {
            _snapScrollView.Page = 1;
            _snapScrollView.RefreshPage();
        }

        /// <summary>
        /// 全難易度の画面を描画する
        /// </summary>
        private void Draw()
        {
            foreach (ELevelName levelName in Enum.GetValues(typeof(ELevelName))) StartCoroutine(DrawEach(levelName));
        }

        /// <summary>
        /// 各難易度の画面を描画する
        /// </summary>
        /// <param name="levelName"> 難易度 </param>
        private IEnumerator DrawEach(ELevelName levelName)
        {
            // GameObject の準備
            GetGameObjects(levelName);
            // 成功割合の描画
            DrawPercentage(levelName);
            // 棒グラフの描画
            DrawGraph(levelName);

            yield return null;
        }

        /// <summary>
        /// 各難易度に必要な GameObject を取得
        /// </summary>
        /// <param name="levelName"> 難易度 </param>
        private void GetGameObjects(ELevelName levelName)
        {
            // Canvas -> SnapScrollView -> Viewport -> Content -> ${levelName} を取得
            var level = GameObject.Find(levelName.ToString()).transform;
            // ${levelName} -> Percentage を取得
            _percentageText.Add(levelName, level.Find("Percentage").gameObject);
            // ${levelName} -> GraphArea を取得
            _graphArea.Add(levelName, level.Find("GraphArea").gameObject);
        }

        /// <summary>
        /// 難易度に合わせた成功割合を描画する
        /// </summary>
        /// <param name="levelName"> 難易度 </param>
        private void DrawPercentage(ELevelName levelName)
        {
            var stageNum = LevelInfo.NUM[levelName];
            var stageStartId = LevelInfo.STAGE_START_ID[levelName];

            var successStageNum = 0;

            // TODO: 全ての記録画面を表示する
            /*
            for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                var stageStatus = StageStatus.Get(stageId);

                if (stageStatus.passed) {
                    successStageNum++;
                }
            }
            */

            var successPercentage = (successStageNum / (float) stageNum) * 100;

            _percentageText[levelName].GetComponent<Text>().text = successPercentage + "%";
        }

        /// <summary>
        /// 難易度に合わせた棒グラフを描画する
        /// </summary>
        /// <param name="levelName"> 難易度 </param>
        private void DrawGraph(ELevelName levelName)
        {
            var stageNum = LevelInfo.NUM[levelName];
            var stageStartId = LevelInfo.STAGE_START_ID[levelName];

            // 描画するボトル
            var graphAreaContent = _graphArea[levelName].GetComponent<RectTransform>();

            // グラフ間の隙間の横幅 -> stageNum個のグラフと(stageNum + 1)個の隙間
            var blankWidth = (_RIGHT_POSITION - _LEFT_POSITION) / (stageNum * _GRAPH_WIDTH_RATIO + (stageNum + 1));

            // 棒グラフの横幅
            var graphWidth = blankWidth * _GRAPH_WIDTH_RATIO;

            // 挑戦回数の最大値を求める
            var maxChallengeNum = GetMaxChallengeNum(stageNum, stageStartId);

            // 挑戦回数の最大値に収まる 30 で割れる目盛の最小値を求める
            var maxScale = (float) Math.Ceiling((float) maxChallengeNum / 30) * 30;

            // 目盛を書き換える
            if (maxScale > 0) {
                _graphArea[levelName].transform.Find("Scale4-Value").gameObject.GetComponent<Text>().text = maxScale.ToString();
                _graphArea[levelName].transform.Find("Scale3-Value").gameObject.GetComponent<Text>().text = (maxScale * 2 / 3).ToString();
                _graphArea[levelName].transform.Find("Scale2-Value").gameObject.GetComponent<Text>().text = (maxScale / 3).ToString();
            }

            // ステージ番号
            var stageName = 1;
            // 描画する棒グラフの左端を示す
            var left = _LEFT_POSITION;

            for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                // 左端と右端の更新
                left += blankWidth;
                var right = left + graphWidth;

                /*
                var stageStatus = StageStatus.Get(stageId);

                // ステージ番号の配置
                var stageNumUi = Instantiate(_stageNumPrefab, graphAreaContent, false);
                stageNumUi.GetComponent<Text>().text = stageName.ToString();
                stageNumUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, _BOTTOM_STAGE_NUM_POSITION);
                stageNumUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, _BOTTOM_POSITION);

                // 棒グラフの配置
                var graphUi = Instantiate(_graphPrefab, graphAreaContent, false);

                // 挑戦回数に応じた棒グラフの上端
                var graphMaxY = _BOTTOM_POSITION;

                if (maxChallengeNum != 0) {
                    // 挑戦回数を用いて，棒グラフの高さを描画範囲に正規化する
                    graphMaxY = (_TOP_POSITION - _BOTTOM_POSITION) * (stageStatus.challengeNum / maxScale) + _BOTTOM_POSITION;
                }

                graphUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, _BOTTOM_POSITION);
                graphUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, graphMaxY);

                if (stageStatus.passed) {
                    // 成功している場合は，色を水色にして，成功した際の挑戦回数も示す
                    graphUi.GetComponent<Image>().color = _successGraphColor;

                    var successLineUi = Instantiate(_successLinePrefab, graphAreaContent, false);
                    var successY = (_TOP_POSITION - _BOTTOM_POSITION) * (stageStatus.firstSuccessNum / maxScale) +
                        _BOTTOM_POSITION;
                    successLineUi.GetComponent<RectTransform>().anchorMin = new Vector2(left, successY);
                    successLineUi.GetComponent<RectTransform>().anchorMax = new Vector2(right, successY);
                } else {
                    // 成功していない場合は，色を赤色にする
                    graphUi.GetComponent<Image>().color = _notSuccessGraphColor;
                }
                */

                // 左端の更新
                left = right;
                // ステージ番号の更新
                stageName++;
            }
        }

        /// <summary>
        /// 挑戦回数の最大値を求める
        /// </summary>
        /// <param name="stageNum"></param>
        /// <param name="stageStartId"></param>
        /// <returns></returns>
        private static int GetMaxChallengeNum(int stageNum, int stageStartId)
        {
            var maxChallengeNum = 0;

            /*
            for (var stageId = stageStartId; stageId < stageStartId + stageNum; stageId++) {
                var stageStatus = StageStatus.Get(stageId);

                if (stageStatus.challengeNum > maxChallengeNum) {
                    maxChallengeNum = stageStatus.challengeNum;
                }
            }
            */

            return maxChallengeNum;
        }
    }
}
