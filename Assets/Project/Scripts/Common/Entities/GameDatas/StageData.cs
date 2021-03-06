using System;
using System.Collections.Generic;
using System.Linq;
using Treevel.Common.Utils;
using UnityEngine;

namespace Treevel.Common.Entities.GameDatas
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
        [SerializeField] private List<string> constraintStages;

        public ETreeId TreeId => treeId;

        public int StageNumber => stageNumber;

        public List<TileData> TileDatas => tiles;

        public List<BottleData> BottleDatas => bottles;

        public List<GimmickData> GimmickDatas => gimmicks;

        public List<EGimmickType> OverviewGimmicks => overviewGimmicks;

        public TutorialData Tutorial => tutorial;

        public List<string> ConstraintStages => constraintStages;

        /// <summary>
        /// ステージIDを取得
        /// </summary>
        public string StageId => EncodeStageIdKey(treeId, stageNumber);

        /// <summary>
        /// ステージのkeyを生成する
        /// </summary>
        /// <param name="treeId"></param>
        /// <param name="stageNumber"></param>
        /// <returns> StageKey(= treeId_stageNumber) </returns>
        public static string EncodeStageIdKey(ETreeId treeId, int stageNumber)
        {
            return $"{treeId.GetTreeIdAsKey()}{Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR}{stageNumber}";
        }

        /// <summary>
        /// ステージのkeyからステージ情報を返す
        /// </summary>
        /// <param name="stageId"></param>
        /// <returns> (treeId, stageNumber) </returns>
        public static (ETreeId, int) DecodeStageIdKey(string stageId)
        {
            var retValues = stageId.Split(Constants.PlayerPrefsKeys.KEY_CONNECT_CHAR);
            if (retValues.Length != 3) throw new Exception("Wrong key format");
            try {
                var treeIdStr = $"{retValues[0]}_{retValues[1]}";
                var treeId = (ETreeId)Enum.Parse(typeof(ETreeId), treeIdStr);
                var stageNumber = int.Parse(retValues[2]);
                return (treeId, stageNumber);
            } catch (Exception e) {
                throw e;
            }
        }
    }
}
