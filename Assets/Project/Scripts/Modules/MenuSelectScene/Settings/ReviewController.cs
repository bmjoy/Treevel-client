using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Project.Scripts.MenuSelectScene.Settings
{
    public class ReviewController : MonoBehaviour
    {
        #if UNITY_IOS
        // TODO: 適切な iOS の ID を設定 (現状，前作の ID を使用)
        private const string _IOS_ID = "1281004285";
        #elif UNITY_ANDROID
        // TODO: 適切な Android の ID を設定
        private const string _ANDROID_ID = "hoge";
        #endif


        /// <summary>
        /// レビューボタンを押した場合の処理
        /// </summary>
        public void ReviewButtonDown()
        {
            // iOS の場合
            #if UNITY_IOS
            if (!Device.RequestStoreReview()) {
                // アプリ内レビューができない場合（iOS10.3以上でなければ発生)
                Application.OpenURL($"itms-apps://itunes.apple.com/jp/app/id{_IOS_ID}?mt=8&action=write-review");
            }
            // Android の場合
            #elif UNITY_ANDROID
            Application.OpenURL($"market://details?id={_ANDROID_ID}");
            #endif
        }
    }
}
