using System.Collections.Generic;
using Project.Scripts.Utils.Definitions;

namespace Project.Scripts.GameDatas
{
    [System.Serializable]
    public class GimmickData
    {
        /// <summary>
        /// ループするかどうか
        /// </summary>
        public bool loop;

        /// <summary>
        /// 登場時間（秒）
        /// </summary>
        public float appearTime;

        /// <summary>
        /// ループする際の登場間隔
        /// </summary>
        public float interval;

        /// <summary>
        /// ギミックの種類
        /// </summary>
        public EGimmickType type;

        /// <summary>
        /// ＜竜巻＞攻撃方向リスト
        /// </summary>
        public List<ETornadoDirection> targetDirections;

        /// <summary>
        /// ＜竜巻＞攻撃行列リスト
        /// </summary>
        public List<int> targetLines;

        /// <summary>
        /// ＜竜巻＞ランダム攻撃方向重みリスト
        /// </summary>
        public List<int> randomDirection;

        /// <summary>
        /// ＜竜巻｜隕石＞ランダム攻撃行重みリスト
        /// </summary>
        public List<int> randomRow;

        /// <summary>
        /// ＜竜巻｜隕石＞ランダム攻撃列重みリスト
        /// </summary>
        public List<int> randomColumn;

        /// <summary>
        /// ＜隕石＞目標行
        /// </summary>
        public ERow targetRow;

        /// <summary>
        /// ＜隕石＞目標列
        /// </summary>
        public EColumn targetColumn;

        /// <summary>
        /// ＜隕石＞追従するボトル
        /// </summary>
        public int targetBottle;

        /// <summary>
        /// ＜隕石＞ランダムに追従するボトルの重み行列
        /// </summary>
        public List<int> randomAttackableBottles;
    }
}
