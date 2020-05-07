namespace Project.Scripts.UIComponents
{
    /// <summary>
    /// ステータスバーの表示を行う
    /// </summary>
    class StatusBarController
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
    }
}
