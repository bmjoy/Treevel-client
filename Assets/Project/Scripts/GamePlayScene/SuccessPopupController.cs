using System;
using System.Collections;
using System.IO;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Library;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Project.Scripts.GamePlayScene
{
    public class SuccessPopupController : MonoBehaviour
    {
        /// <summary>
        /// 次に進むボタン
        /// </summary>
        private GameObject _nextButton;

        /// <summary>
        /// 投稿ボタン
        /// </summary>
        private GameObject _shareButton;

        private void Awake()
        {
            GamePlayDirector.OnSucceed += OnSucceed;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GamePlayDirector.OnSucceed -= OnSucceed;
        }

        private void OnSucceed()
        {
            gameObject.SetActive(true);
        }

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
            // Unity エディタ上では実行しない
            #if !UNITY_EDITOR
            StartCoroutine(Share());
            #endif
        }

        private IEnumerator Share()
        {
            var title = transform.Find("Title").gameObject.GetComponent<MultiLanguageText>().text;

            // 投稿用のテキスト
            var text = "ステージ" + GamePlayDirector.stageId + "番を" + title;
            // URL 用に加工
            text = UnityWebRequest.EscapeURL(text);

            // 投稿用のハッシュタグ
            var hashTags = "NumberBullet,ナンバレ";
            hashTags = UnityWebRequest.EscapeURL(hashTags);

            // スクリーンショットを撮る
            const string imgPath = "Assets/StreamingAssets/SuccessScreenShot.png";
            ScreenCapture.CaptureScreenshot(imgPath);

            // スクリーンショットが保存されるのを待つ
            while (!File.Exists(imgPath)) {
                yield return null;
            }

            // シェア画面へ
            SocialConnector.SocialConnector.Share(text, "", imgPath);
        }
    }
}
