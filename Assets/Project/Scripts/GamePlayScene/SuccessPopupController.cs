using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.Networking;

namespace Project.Scripts.GamePlayScene
{
    public class SuccessPopupController : MonoBehaviour
    {
        /// <summary>
        /// 次に進むボタン押下時の処理
        /// </summary>
        public void NextButtonDown()
        {
            // StageSelectSceneに戻る
            TreeLibrary.LoadStageSelectScene(GamePlayDirector.levelName);
        }

        /// <summary>
        /// 投稿ボタン押下時の処理
        /// </summary>
        public void ShareButtonDown()
        {
            // 投稿用のテキスト
            var text = "ステージ" + GamePlayDirector.stageId + "番をクリア！";
            text = UnityWebRequest.EscapeURL(text);

            // 投稿用のハッシュタグ
            var hashTags = "NumberBullet,ナンバレ";
            hashTags = UnityWebRequest.EscapeURL(hashTags);

            // Twitter 投稿画面へ
            Application.OpenURL("https://twitter.com/intent/tweet?text=" + text + "&hashtags=" + hashTags);
        }
    }
}
