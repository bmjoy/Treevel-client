using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Project.Scripts.MenuSelectScene
{
    /// <summary>
    /// 全体ゲームに出現するUI（プログレスバー、メッセージダイアログ等）
    /// を制御するマネージャークラス
    /// </summary>
    public class UIManager : SingletonObject<UIManager>
    {
        /// <summary>
        /// プログレスバーのプレハブ
        /// </summary>
        [SerializeField]
        private AssetReferenceGameObject _progressBar;

        /// <summary>
        /// プログレスバーのインスタンス
        /// </summary>
        public ProgressBar ProgressBar
        {
            get;
            private set;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        private void Start()
        {
            // キャンバスがなければ作る
            if (GetComponentInChildren<Canvas>() == null) {
                gameObject.AddComponent<Canvas>();
            }

            var canvas = GetComponentInChildren<Canvas>().transform;

            // キャンバスの下にプログレスバーの実体を生成
            _progressBar.InstantiateAsync(canvas).Completed += (obj) => {
                ProgressBar = obj.Result.GetComponentInChildren<ProgressBar>();
            };
        }
    }
}
