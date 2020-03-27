using System.Collections;
using Project.Scripts.UIComponents;
using Project.Scripts.Utils.Patterns;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        public ProgressBar ProgressBar { get; private set; }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);    
        }

        // Start is called before the first frame update
        void Start()
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

        /// <summary>
        /// プログレスバーを表示／非表示
        /// </summary>
        /// <param name="show"></param>
        public void ShowProgressBar(bool show)
        {
            ProgressBar.gameObject.SetActive(show);
        }

        /// <summary>
        /// 受け取った<code>AsyncOperationHandle</code>に基づきプログレスを表示する
        /// </summary>
        /// <param name="handle">監視するプロセス</param>
        public void ShowProgress(AsyncOperationHandle handle)
        {
            ShowProgressBar(true);
            StartCoroutine(ShowProgress_Impl(handle));
        }

        private IEnumerator ShowProgress_Impl(AsyncOperationHandle handle)
        {
            while (!handle.IsDone) {
                ProgressBar.Progress = handle.PercentComplete;
                yield return null;
            }
            ShowProgressBar(false);
        }
    }
}
