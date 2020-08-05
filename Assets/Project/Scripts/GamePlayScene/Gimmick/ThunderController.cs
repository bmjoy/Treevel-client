using System.Collections;
using UnityEngine;
using Project.Scripts.Utils.Definitions;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Utils.PlayerPrefsUtils;

namespace Project.Scripts.GamePlayScene.Gimmick
{
    public class ThunderController : AbstractGimmickController
    {
        /// <summary>
        /// 雲をタイルの比率でオフセットさせる
        /// </summary>
        private static readonly float _CLOUD_OFFSET_BY_TILE_RATIO = 0.6f;

        /// <summary>
        /// Idleステートのハッシュ値
        /// </summary>
        private static int IDLE_STATE_NAME_HASH = Animator.StringToHash("Idle");

        private static int ENTRY_STATE_NAME_HASH = Animator.StringToHash("Entry");

        /// <summary>
        /// 雷を放つ際再生するSE
        /// </summary>
        [SerializeField] private AudioClip _attackSE;

        /// <summary>
        /// 移動速度
        /// </summary>
        [SerializeField] private float _moveSpeed = 1.5f;

        /// <summary>
        /// 目標タイル
        /// </summary>
        private List<(ERow row, EColumn column)> _targets;

        /// <summary>
        /// 雲オブジェクト
        /// </summary>
        private Transform _cloud;


        // 各コンポーネント
        private Animator _animator;
        private AudioSource _audioSource;
        private Rigidbody2D _rigidBody;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _rigidBody = GetComponent<Rigidbody2D>();

            // SE音量を適用
            _audioSource.volume *= UserSettings.SEVolume;

            // 雲をタイルの少し上に移動する
            _cloud = transform.Find("Cloud");
            _cloud.Translate(0, _CLOUD_OFFSET_BY_TILE_RATIO * TileSize.HEIGHT, 0, Space.Self);
        }

        public override void Initialize(GameDatas.GimmickData gimmickData)
        {
            base.Initialize(gimmickData);

            _targets = gimmickData.targets
                // 無効な値を除外する
                .SkipWhile((vec) => vec.x < (int)ERow.First || (int)ERow.Fifth < vec.x || vec.y < (int)EColumn.Left || (int)EColumn.Right < vec.y)
                // List<(ERow, EColumn)>に変換する
                .Select((vec) => ((ERow)vec.x, (EColumn)vec.y)).ToList();

            Debug.Assert(_targets.Count > 0, "Invalid Gimmick Data");

            // 初期位置に
            var initPos = BoardManager.Instance.GetTilePos(_targets[0].row, _targets[0].column);
            transform.position = initPos;

            // 描画順調整
            _cloud.GetComponent<Renderer>().sortingOrder++;
        }

        public override IEnumerator Trigger()
        {
            // 表示ON
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>()) {
                spriteRenderer.enabled = true;
            }

            var targetEnumerator = _targets.GetEnumerator();
            while (targetEnumerator.MoveNext()) {
                var targetPos = BoardManager.Instance.GetTilePos(targetEnumerator.Current.row, targetEnumerator.Current.column);

                // 移動ベクトル設定
                _rigidBody.velocity = (targetPos - (Vector2)transform.position).normalized * _moveSpeed;

                // 目標地まで移動する
                yield return new WaitUntil(() => Vector2.Dot(targetPos - (Vector2)transform.position, _rigidBody.velocity) <= 0);

                // 移動停止
                _rigidBody.velocity = Vector2.zero;

                // 警告 -> 攻撃
                _animator.SetTrigger("Warning");

                // すぐには遷移してくれないそうなので、次のステート（警告）に遷移するまでちょっと待つ
                yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash != IDLE_STATE_NAME_HASH);

                // 攻撃アニメーション終わるまで待つ
                yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == IDLE_STATE_NAME_HASH && !_audioSource.isPlaying);
            }

            // TODO:退場演出
            Destroy(gameObject);
        }

        /// <summary>
        /// 攻撃する（アニメーションイベントから呼び出す）
        /// </summary>
        private void Attack()
        {
            _audioSource.PlayOneShot(_attackSE);
        }
    }
}
