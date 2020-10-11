using Project.Scripts.Utils.Definitions;
using Project.Scripts.Utils.PlayerPrefsUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.GameDatas
{
    [CreateAssetMenu(fileName = "stage.asset", menuName = "Stage")]
    public class StageData : ScriptableObject
    {
        [SerializeField] private ETreeId treeId;
        [SerializeField] private int stageNumber;
        [SerializeField] private List<TileData> tiles;
        [SerializeField] private List<BottleData> bottles;
        [SerializeField] private List<GimmickData> gimmicks;
        [SerializeField] private List<EGimmickType> overviewGimmicks;
        [SerializeField] private TutorialData tutorial;

        public ETreeId TreeId => treeId;

        public int StageNumber => stageNumber;

        public List<TileData> TileDatas => tiles;

        public List<BottleData> BottleDatas => bottles;

        public List<GimmickData> GimmickDatas => gimmicks;

        public List<EGimmickType> OverviewGimmicks => overviewGimmicks;

        public TutorialData Tutorial => tutorial;

        /// <summary>
        /// ステージのkeyを生成する
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="stageNumber"></param>
        /// <returns> StageKey(= treeId_stageNumber) </returns>
        public static string EncodeStageIdKey(ETreeId treeId, int stageNumber)
        {
            return $"{treeId}{PlayerPrefsKeys.KEY_CONNECT_CHAR}{stageNumber}";
        }

        /// <summary>
        /// ステージのkeyからステージ情報を返す
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns> (treeId, stageNumber) </returns>
        public static(ETreeId, int) DecodeStageIdKey(string stageId)
        {
            var retValues = stageId.Split(PlayerPrefsKeys.KEY_CONNECT_CHAR);
            if (retValues.Length != 2) throw new Exception("Wrong key format");
            try {
                var treeId = (ETreeId) Enum.ToObject(typeof(ETreeId), retValues[0]);
                var stageNumber = int.Parse(retValues[1]);
                return (treeId, stageNumber);
            } catch (Exception e) {
                throw e;
            }
        }
    }
}
