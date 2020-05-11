using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Project.Scripts.UIComponents
{
    /// <summary>
    /// ステータスバーの表示を行う
    /// </summary>
    public static class StatusBarController
    {
        private static int _FLAG_FULLSCREEN = 0x00000400;
        private static int _FLAG_FORCE_NOT_FULLSCREEN = 0x00000800;

        public static void Show()
        {
            #if !UNITY_EDITOR && UNITY_ANDROID
            // フルスクリーン表示をやめる
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                    // 別スレッド
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(RunOnUiThread));
                }
            }
            #endif
        }

        /// <summary>
        /// フルスクリーン表示をやめるフラッグを管理する
        /// </summary>
        private static void RunOnUiThread()
        {
            #if !UNITY_EDITOR && UNITY_ANDROID
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                    using (var window = activity.Call<AndroidJavaObject>("getWindow")) {
                        window.Call("clearFlags", _FLAG_FULLSCREEN);
                        window.Call("addFlags", _FLAG_FORCE_NOT_FULLSCREEN);
                    }
                }
            }
            #endif
        }

        /// <summary>
        /// ステータスバーの下側のアンカーを取得する
        /// </summary>
        /// <returns> ステータスバーの高さを取得する</returns>
        public static float GetStatusBarBottomAnchor()
        {
            #if UNITY_IOS || UNITY_EDITOR
            var safeArea = Screen.safeArea;
            if (Screen.height == Screen.safeArea.height) {
                // iPhone X以前の機種
                var deviceGeneration = UnityEngine.iOS.Device.generation;
                if (deviceGeneration == UnityEngine.iOS.DeviceGeneration.iPhone6Plus ||
                    deviceGeneration == UnityEngine.iOS.DeviceGeneration.iPhone6SPlus ||
                    deviceGeneration == UnityEngine.iOS.DeviceGeneration.iPhone7Plus ||
                    deviceGeneration == UnityEngine.iOS.DeviceGeneration.iPhone8Plus) {
                    // 高さは54px
                    return 1f - (54f / Screen.height);
                } else {
                    // 高さは40px
                    return 1f - (40f / Screen.height);
                }
            } else {
                // SafeAreaの上側のアンカーの位置を返す
                var anchorMax = safeArea.position + safeArea.size;
                return anchorMax.y / Screen.height;
            }
            #elif UNITY_ANDROID
            // ステータスバーの高さを取得する
            var rectangle = new Rect();
            var window = getWindow();
            window.getDecorView().getWindowVisibleDisplayFrame(rectangle);
            var statusBarHeight = rectangle.top;
            return 1f - (float)statusBarHeight / Screen.height;
            #endif
        }
    }
}
