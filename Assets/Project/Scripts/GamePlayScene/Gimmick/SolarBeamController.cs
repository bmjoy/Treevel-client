using System.Collections;
using Project.Scripts.GameDatas;
using Project.Scripts.Utils.Definitions;
using UnityEngine;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    [RequireComponent(typeof(Animator))]
    public class SolarBeamController : AbstractGimmickController
    {
        [SerializeField] private GameObject _sunObject;

        [SerializeField] private GameObject _beamObject;

        /// <summary>
        /// 登場から警告出すまでの時間
        /// </summary>
        [SerializeField] private float _idleTime = 2.0f;

        private const string _WARNING_TRIGGER_NAME = "Warning";

        private Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public override void Initialize(GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            var direction = gimmickData.solarBeamDirection;
            switch (direction) {
                case EGimmickDirection.ToLeft:
                case EGimmickDirection.ToRight: {
                        var row = gimmickData.targetRow;

                        // 親オブジェクトの位置設定
                        var initialPos = BoardManager.Instance.GetTilePos(row, EColumn.Center);
                        transform.position = initialPos;

                        // 太陽とビームの位置を調整する
                        var sunRenderer = _sunObject.GetComponent<SpriteRenderer>();
                        // 中央から1.5タイルサイズ＋1.5太陽の幅分ずらす
                        var sign = direction == EGimmickDirection.ToLeft ? 1 : -1;
                        var offset = new Vector2(TileSize.WIDTH * 1.5f + sunRenderer.size.x * 1.5f, 0);
                        _sunObject.transform.position = initialPos + sign * offset;
                        // ToRightの場合はx反転
                        _sunObject.transform.localScale = Vector3.Scale(_sunObject.transform.localScale, new Vector3(sign, 1, 1));
                        sunRenderer.enabled = true;

                        break;
                    }
                case EGimmickDirection.ToUp:
                case EGimmickDirection.ToBottom: {
                        var col = gimmickData.targetColumn;

                        // 親オブジェクトの位置設定
                        var initialPos = BoardManager.Instance.GetTilePos(ERow.Third, col);
                        transform.position = initialPos;

                        // 太陽とビームの位置を調整する
                        var sunRenderer = _sunObject.GetComponent<SpriteRenderer>();
                        var sign = direction == EGimmickDirection.ToBottom ? 1 : -1;
                        var offset = new Vector2(0, TileSize.HEIGHT * 2.5f + sunRenderer.size.y * 1.5f);
                        _sunObject.transform.position = initialPos + sign * offset;
                        sunRenderer.enabled = true;

                        _beamObject.transform.Rotate(Quaternion.Euler(0, 0, sign * 90).eulerAngles);
                        var beamSunDistance = Vector3.Distance(_beamObject.transform.position, _sunObject.transform.position);
                        _beamObject.transform.position = _sunObject.transform.position + sign * Vector3.down * beamSunDistance;
                        break;
                    }
            }
        }

        public override IEnumerator Trigger()
        {
            yield return new WaitForSeconds(_idleTime);

            _animator.SetTrigger(_WARNING_TRIGGER_NAME);
        }
    }
}
