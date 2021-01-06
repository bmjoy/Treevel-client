using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Treevel.Common.Entities;
using Treevel.Common.Entities.GameDatas;
using Treevel.Common.Managers;
using Treevel.Common.Patterns.Singleton;
using Treevel.Common.Utils;
using UniRx;
using UnityEngine;

namespace Treevel.Modules.GamePlayScene.Gimmick
{
    public class GimmickGenerator : SingletonObject<GimmickGenerator>
    {
        private List<GimmickData> _gimmicks;

        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// FireGimmickが呼ばれた時刻
        /// </summary>
        private float _startTime;

        private readonly Dictionary<EGimmickType, string> _prefabAddressableKeys = new Dictionary<EGimmickType, string>()
        {
            {EGimmickType.Tornado, Constants.Address.TORNADO_PREFAB},
            {EGimmickType.Meteorite, Constants.Address.METEORITE_PREFAB},
            {EGimmickType.AimingMeteorite, Constants.Address.AIMING_METEORITE_PREFAB},
            {EGimmickType.Thunder, Constants.Address.THUNDER_PREFAB},
            {EGimmickType.SolarBeam, Constants.Address.SOLAR_BEAM_PREFAB},
            {EGimmickType.GustWind, Constants.Address.GUST_WIND_PREFAB},
            {EGimmickType.Fog, Constants.Address.FOG_PREFAB},
            {EGimmickType.Powder, Constants.Address.POWDER_PREFAB},
        };

        public UniTask Initialize(List<GimmickData> gimmicks)
        {
            _tokenSource = new CancellationTokenSource();
            _gimmicks = gimmicks;
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// ギミック発動
        /// </summary>
        public void FireGimmick()
        {
            _startTime = Time.time;

            _gimmicks.Select(data => CreateGimmickCoroutine(_tokenSource.Token, data)).ToList().ForEach(task => task.Forget());
        }

        /// <summary>
        /// ギミックの実体を生成するコルチーン
        /// </summary>
        /// <param name="data">生成するギミックのデータ</param>
        private async UniTask CreateGimmickCoroutine(CancellationToken token, GimmickData data)
        {
            // 出現時間経つまで待つ
            await UniTask.DelayFrame(Math.Max(1, (int)(GamePlayDirector.FRAME_RATE * (data.appearTime - (Time.time - _startTime)))));

            do {
                if (token.IsCancellationRequested) return;

                var instantiateTime = Time.time;
                // ギミックインスタンス作成
                var gimmickObject = await AddressableAssetManager.Instantiate(_prefabAddressableKeys[data.type]).ToUniTask();

                if (gimmickObject == null) {
                    Debug.LogError($"ギミックの生成が失敗しました。");
                    return;
                }

                var gimmick = gimmickObject.GetComponent<AbstractGimmickController>();
                if (gimmick == null) {
                    Debug.LogError($"ギミックコントローラが{gimmickObject.name}にアタッチされていません。");
                    return;
                }

                // 初期化
                gimmick.Initialize(data);

                // ギミック発動
                gimmick.Trigger(token);

                // ギミック発動間隔
                await UniTask.DelayFrame(Math.Max(1, (int)(GamePlayDirector.FRAME_RATE * (data.interval - (Time.time - instantiateTime)))));
            } while (data.loop);
        }

        private void OnEnable()
        {
            Observable.Merge(GamePlayDirector.Instance.GameSucceeded, GamePlayDirector.Instance.GameFailed)
            .Subscribe(_ => {
                OnGameEnd();
            }).AddTo(this);
        }

        private void OnGameEnd()
        {
            // 全てのGimmickを停止させる
            _tokenSource.Cancel();
        }
    }
}
