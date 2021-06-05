using Treevel.Common.Utils;
using UnityEngine;
using UnityEngine.Playables;

namespace Treevel.Modules.MenuSelectScene.LevelSelect
{
    /// <summary>
    /// レベルセレクトシーンのロードを制御するためのカスタマイズトラックアセット
    /// </summary>
    public class RoadPlayableAsset : PlayableAsset
    {
        /// <summary>
        /// Playableが再生された時（Playが呼ばれた時）に呼ばれる関数
        /// ここで変数をカスタマイズトラックに渡すことができる
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="owner">このPlayableAssetを使っているPlayable Directorがアタッチしているゲームオブジェクト</param>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // PlayableBehaviourを元にPlayableを作る
            var playable = ScriptPlayable<RoadPlayableBehaviour>.Create(graph);
            // PlayableBehaviourを取得する
            var behaviour = playable.GetBehaviour();
            // 参照を解決する
            behaviour.roadController = (RoadController) graph.GetResolver().GetReferenceValue(Constants.TimelineReferenceKey.ROAD_TO_RELEASE, out var isValid);
            Debug.Assert(isValid, $"Unable to resolve reference {Constants.TimelineReferenceKey.ROAD_TO_RELEASE}");

            return playable;
        }
    }
}
