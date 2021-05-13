using GoogleMobileAds.Api;
using UnityEngine;

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
            var adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            var view = position switch {
                AdPosition.Top => new BannerView(unitId, adSize, (int) Screen.safeArea.x, -(int) Screen.safeArea.y),
                _ => new BannerView(unitId, adSize, position)
            };

            var request = new AdRequest.Builder().Build();

            view.LoadAd(request);
            return view;
        }
    }
}
