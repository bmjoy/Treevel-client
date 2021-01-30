using Treevel.Common.Entities;
using Treevel.Common.Managers;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene
{
    public class FailurePopupController : MonoBehaviour
    {
        /// <summary>
        /// リトライボタン押下時の処理
        /// </summary>
        public void RetryButtonDown()
        {
            GamePlayDirector.Instance.Dispatch(GamePlayDirector.EGameState.Opening);
        }

        /// <summary>
        /// 戻るボタン押下時の処理
        /// </summary>
        public void BackButtonDown()
        {
            // StageSelectSceneに戻る
            AddressableAssetManager.LoadScene(GamePlayDirector.seasonId.GetSceneName());
        }
    }
}
