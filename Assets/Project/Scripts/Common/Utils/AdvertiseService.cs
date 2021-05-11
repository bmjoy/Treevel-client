using GoogleMobileAds.Api;

namespace Treevel.Common.Utils
{
    /// <summary>
    /// 広告用のユーティリティクラス
    /// </summary>
    public static class AdvertiseService
    {
        /// <summary>
        /// バナーを表示する
        /// </summary>
        /// <param name="unitId">AdMobで発行された広告ユニットID</param>
        /// <param name="position">バナーを表示する位置</param>
        /// <returns>バナーのビューインスタンス</returns>
        public static BannerView ShowBanner(string unitId, AdPosition position)
        {
            #if UNITY_EDITOR
            // ダミー広告はあらかじめ用意されたサイズしか指定できない
            var view = new BannerView(unitId, AdSize.SmartBanner, position);
            #elif UNITY_ANDROID || UNITY_IOS
            // TODO 実機で動作、見た目確認
            var width = Screen.width;
            var height = (int)(Screen.height * 0.1);

            var view = new BannerView(unitId, new AdSize(width, height), position);
            #endif
            var request = new AdRequest.Builder().Build();

            view.LoadAd(request);
            return view;
        }
    }
}
